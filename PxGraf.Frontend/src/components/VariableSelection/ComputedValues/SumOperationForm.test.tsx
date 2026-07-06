import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { SumOperationForm } from './SumOperationForm';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimensionValue } from 'types/cubeMeta';

const availableValues: IDimensionValue[] = [
    { code: '2018', name: { en: '2018', fi: '2018', sv: '2018' }, isVirtual: false },
    { code: '2019', name: { en: '2019', fi: '2019', sv: '2019' }, isVirtual: false },
    { code: '2020', name: { en: '2020', fi: '2020', sv: '2020' }, isVirtual: false },
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
            <SumOperationForm
                availableValues={availableValues}
                operandCodes={operandCodes}
                constant={constant}
                onChange={onChange}
            />
        </UiLanguageContext.Provider>
    );

describe('SumOperationForm', () => {
    it('renders the values autocomplete', () => {
        renderForm();
        expect(screen.getByLabelText('computedValues.selectValues')).toBeInTheDocument();
    });

    it('hides the constant field by default', () => {
        renderForm();
        expect(screen.queryByLabelText('computedValues.constant')).not.toBeInTheDocument();
    });

    it('shows constant field after toggling the switch', async () => {
        const user = userEvent.setup();
        renderForm();
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        expect(screen.getByLabelText('computedValues.constant')).toBeInTheDocument();
    });

    it('calls onChange when constant is changed', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm([], undefined, mockOnChange);
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        const constantInput = screen.getByLabelText('computedValues.constant');
        fireEvent.change(constantInput, { target: { value: '5' } });
        expect(mockOnChange).toHaveBeenCalledWith([], 5);
    });

    it('calls onChange with operand codes when values are selected', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm([], undefined, mockOnChange);
        const autocompleteInput = screen.getByLabelText('computedValues.selectValues');
        await user.click(autocompleteInput);
        const option = await screen.findByRole('option', { name: '2018' });
        await user.click(option);
        expect(mockOnChange).toHaveBeenCalledWith(['2018'], undefined);
    });
});
