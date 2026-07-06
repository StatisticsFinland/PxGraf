import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { DivisionOperationForm } from './DivisionOperationForm';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimensionValue } from 'types/cubeMeta';

const availableValues: IDimensionValue[] = [
    { code: '2018', name: { en: '2018', fi: '2018', sv: '2018' }, isVirtual: false },
    { code: '2019', name: { en: '2019', fi: '2019', sv: '2019' }, isVirtual: false },
];

const uiLanguageContextValue = {
    language: 'en',
    setLanguage: jest.fn(),
    languageTab: 'en',
    setLanguageTab: jest.fn(),
    availableUiLanguages: ['en', 'fi', 'sv'],
    uiContentLanguage: 'en',
    setUiContentLanguage: jest.fn(),
};

const renderForm = (
    operandCodes: string[] = [],
    constant: number | undefined = undefined,
    onChange = jest.fn(),
) =>
    render(
        <UiLanguageContext.Provider value={uiLanguageContextValue}>
            <DivisionOperationForm
                availableValues={availableValues}
                operandCodes={operandCodes}
                constant={constant}
                onChange={onChange}
            />
        </UiLanguageContext.Provider>
    );

describe('DivisionOperationForm', () => {
    it('renders dividend and divisor selects in value mode', () => {
        renderForm();
        expect(screen.getByText('computedValues.dividendValue', { selector: 'label' })).toBeInTheDocument();
        expect(screen.getByText('computedValues.divisorValue', { selector: 'label' })).toBeInTheDocument();
    });

    it('shows constant field and hides divisor select after toggling to constant mode', async () => {
        const user = userEvent.setup();
        renderForm();
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        expect(screen.getByLabelText('computedValues.constant')).toBeInTheDocument();
        expect(screen.queryByText('computedValues.divisorValue')).not.toBeInTheDocument();
    });

    it('does not show division-by-zero error when constant is non-zero', async () => {
        const user = userEvent.setup();
        renderForm();
        // Enable constant mode; localConstant defaults to 1
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        expect(
            screen.queryByText('computedValues.validationDivisionByZero'),
        ).not.toBeInTheDocument();
    });

    it('shows division-by-zero error when constant is set to 0', async () => {
        const user = userEvent.setup();
        renderForm();
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        const constantInput = screen.getByLabelText('computedValues.constant');
        fireEvent.change(constantInput, { target: { value: '0' } });
        expect(screen.getByText('computedValues.validationDivisionByZero')).toBeInTheDocument();
    });

    it('calls onChange with constant value when constant is changed', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm([], undefined, mockOnChange);
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        const constantInput = screen.getByLabelText('computedValues.constant');
        fireEvent.change(constantInput, { target: { value: '2' } });
        expect(mockOnChange).toHaveBeenCalledWith([], 2);
    });

    it('restores divisor select when switching back to value mode', async () => {
        const user = userEvent.setup();
        renderForm();
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.useValue' }));
        expect(screen.getByText('computedValues.divisorValue', { selector: 'label' })).toBeInTheDocument();
        expect(screen.queryByLabelText('computedValues.constant')).not.toBeInTheDocument();
    });
});
