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

    it('renders both toggle buttons', () => {
        renderForm();
        expect(screen.getByRole('button', { name: 'computedValues.useValue' })).toBeInTheDocument();
        expect(screen.getByRole('button', { name: 'computedValues.constant' })).toBeInTheDocument();
    });

    it('initializes in constant mode when constant prop is provided', () => {
        renderForm(['2018'], 5);
        expect(screen.getByLabelText('computedValues.constant')).toBeInTheDocument();
        expect(screen.getByLabelText('computedValues.constant')).toHaveValue(5);
        expect(screen.queryByText('computedValues.divisorValue', { selector: 'label' })).not.toBeInTheDocument();
    });

    it('calls onChange when toggling to constant mode', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm(['2018'], undefined, mockOnChange);
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        expect(mockOnChange).toHaveBeenCalledWith(['2018'], 1);
    });

    it('calls onChange when toggling back to value mode', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm(['2018'], undefined, mockOnChange);
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        mockOnChange.mockClear();
        await user.click(screen.getByRole('button', { name: 'computedValues.useValue' }));
        expect(mockOnChange).toHaveBeenCalledWith(['2018'], undefined);
    });

    it('defaults constant to 0 when a non-numeric value is entered', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm([], undefined, mockOnChange);
        await user.click(screen.getByRole('button', { name: 'computedValues.constant' }));
        const constantInput = screen.getByLabelText('computedValues.constant');
        fireEvent.change(constantInput, { target: { value: 'abc' } });
        expect(mockOnChange).toHaveBeenCalledWith([], 0);
    });

    it('calls onChange when dividend value is changed', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm([], undefined, mockOnChange);
        const comboboxes = screen.getAllByRole('combobox');
        await user.click(comboboxes[0]);
        const option = await screen.findByRole('option', { name: '2018' });
        await user.click(option);
        expect(mockOnChange).toHaveBeenCalledWith(['2018'], undefined);
    });

    it('calls onChange when divisor value is changed', async () => {
        const user = userEvent.setup();
        const mockOnChange = jest.fn();
        renderForm(['2018'], undefined, mockOnChange);
        const comboboxes = screen.getAllByRole('combobox');
        await user.click(comboboxes[1]);
        const option = await screen.findByRole('option', { name: '2019' });
        await user.click(option);
        expect(mockOnChange).toHaveBeenCalledWith(['2018', '2019'], undefined);
    });
});
