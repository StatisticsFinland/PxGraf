import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { ComputedValuesDialog } from './ComputedValuesDialog';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { FilterType, IDimensionQuery, ISumDefinition, ISubtractionByConstantDefinition, IMultiplicationByConstantDefinition, IDivisionByConstantDefinition } from 'types/query';

const mockDimension: IDimension = {
    code: 'Vuosi',
    name: { en: 'Year', fi: 'Vuosi', sv: 'År' },
    type: EDimensionType.Time,
    values: [
        { code: '2018', name: { en: '2018', fi: '2018', sv: '2018' }, isVirtual: false },
        { code: '2019', name: { en: '2019', fi: '2019', sv: '2019' }, isVirtual: false },
        { code: '2020', name: { en: '2020', fi: '2020', sv: '2020' }, isVirtual: false },
        { code: 'virtual_1', name: { en: 'Computed value 1', fi: 'Laskettu arvo 1', sv: 'Beräknat värde 1' }, isVirtual: true },
    ],
};

const baseDimensionQuery: IDimensionQuery = {
    valueFilter: { type: FilterType.Top, query: 3 },
    selectable: false,
    virtualValueDefinitions: [],
};

const uiLanguageContextValue = {
    language: 'en',
    setLanguage: jest.fn(),
    languageTab: 'en',
    setLanguageTab: jest.fn(),
    availableUiLanguages: ['en', 'fi', 'sv'],
    uiContentLanguage: 'en',
    setUiContentLanguage: jest.fn(),
};

const renderDialog = (
    dimensionQuery = baseDimensionQuery,
    onQueryChanged = jest.fn(),
    onClose = jest.fn(),
) =>
    render(
        <UiLanguageContext.Provider value={uiLanguageContextValue}>
            <ComputedValuesDialog
                open={true}
                dimension={mockDimension}
                dimensionQuery={dimensionQuery}
                onClose={onClose}
                onQueryChanged={onQueryChanged}
            />
        </UiLanguageContext.Provider>
    );

describe('ComputedValuesDialog — list view', () => {
    it('shows no-values text when definitions list is empty', () => {
        renderDialog();
        expect(screen.getByText('computedValues.noValues')).toBeInTheDocument();
    });

    it('shows definition code and operator label when a definition exists', () => {
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDef);
        expect(screen.getByText(/Computed value 1/)).toBeInTheDocument();
        expect(screen.getByText(/computedValues\.operatorSum/)).toBeInTheDocument();
    });
});

describe('ComputedValuesDialog — form view', () => {
    it('switches to form view when Add New is clicked', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        expect(screen.getByRole('button', { name: 'computedValues.operatorSum' })).toBeInTheDocument();
    });

    it('returns to list view when Cancel is clicked from form view', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.cancel' }));
        expect(screen.getByText('computedValues.noValues')).toBeInTheDocument();
    });

    it('shows SubtractionOperationForm when subtraction operator is selected', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorSubtraction' }));
        expect(screen.getByText('computedValues.baseValue', { selector: 'label' })).toBeInTheDocument();
    });

    it('shows DivisionOperationForm when division operator is selected', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorDivision' }));
        expect(screen.getByText('computedValues.dividendValue', { selector: 'label' })).toBeInTheDocument();
    });

    it('prefills the form with sum operator when Edit is clicked on a sum definition', async () => {
        const user = userEvent.setup();
        const definition: ISumDefinition = {
            type: 'sum',
            code: 'virtual_1',
            operandCodes: ['2018', '2019'],
        };
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [definition],
        };
        renderDialog(queryWithDef);
        await user.click(screen.getByRole('button', { name: 'computedValues.edit' }));
        expect(screen.getByRole('button', { name: 'computedValues.operatorSum' })).toHaveAttribute('aria-pressed', 'true');
    });

    it('shows virtual value exactly once in operand list when adding new definition', async () => {
        const user = userEvent.setup();
        const dimensionWithVirtual: IDimension = {
            ...mockDimension,
            values: [
                ...mockDimension.values.filter(v => v.code !== 'virtual_1'),
                { code: 'virtual_1', name: { en: 'My computed value', fi: 'Laskettu arvo', sv: 'Beräknat värde' }, isVirtual: true },
            ],
        };
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        render(
            <UiLanguageContext.Provider value={uiLanguageContextValue}>
                <ComputedValuesDialog
                    open={true}
                    dimension={dimensionWithVirtual}
                    dimensionQuery={queryWithDef}
                    onClose={jest.fn()}
                    onQueryChanged={jest.fn()}
                />
            </UiLanguageContext.Provider>
        );
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        // Click on the Autocomplete to open the dropdown
        const autocompleteInput = screen.getByRole('combobox', { name: /computedValues\.selectValues/ });
        await user.click(autocompleteInput);
        // Count how many times 'My computed value' appears in the dropdown options
        const options = screen.getAllByText('My computed value');
        expect(options).toHaveLength(1);
    });

    it('excludes the virtual value being edited from operand list', async () => {
        const user = userEvent.setup();
        const dimensionWithVirtual: IDimension = {
            ...mockDimension,
            values: [
                ...mockDimension.values.filter(v => v.code !== 'virtual_1'),
                { code: 'virtual_1', name: { en: 'My computed value', fi: 'Laskettu arvo', sv: 'Beräknat värde' }, isVirtual: true },
            ],
        };
        const definition: ISumDefinition = {
            type: 'sum',
            code: 'virtual_1',
            operandCodes: ['2018', '2019'],
        };
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [definition],
        };
        render(
            <UiLanguageContext.Provider value={uiLanguageContextValue}>
                <ComputedValuesDialog
                    open={true}
                    dimension={dimensionWithVirtual}
                    dimensionQuery={queryWithDef}
                    onClose={jest.fn()}
                    onQueryChanged={jest.fn()}
                />
            </UiLanguageContext.Provider>
        );
        await user.click(screen.getByRole('button', { name: 'computedValues.edit' }));
        // Click on the Autocomplete to open the dropdown
        const autocompleteInput = screen.getByRole('combobox', { name: /computedValues\.selectValues/ });
        await user.click(autocompleteInput);
        // Verify 'My computed value' does not appear in the dropdown options
        const options = screen.queryAllByText('My computed value');
        expect(options).toHaveLength(0);
    });
});

describe('ComputedValuesDialog — delete', () => {
    it('calls onQueryChanged without the deleted definition', async () => {
        const user = userEvent.setup();
        const mockOnQueryChanged = jest.fn();
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDef, mockOnQueryChanged);
        await user.click(screen.getByRole('button', { name: 'computedValues.delete' }));
        expect(mockOnQueryChanged).toHaveBeenCalledWith(
            expect.objectContaining({ virtualValueDefinitions: [] }),
        );
    });
});

describe('ComputedValuesDialog — dependency safeguard', () => {
    it('disables Edit and Delete buttons for a value that another virtual value depends on', async () => {
        const queryWithDeps: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
                { type: 'sum', code: 'virtual_2', operandCodes: ['virtual_1', '2020'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDeps);
        // virtual_1 is a dependency of virtual_2, so its buttons should be disabled
        const editButtons = screen.getAllByRole('button', { name: 'computedValues.edit' });
        const deleteButtons = screen.getAllByRole('button', { name: 'computedValues.delete' });
        // First item is virtual_1 (depended on), second is virtual_2 (not depended on)
        expect(editButtons[0]).toBeDisabled();
        expect(deleteButtons[0]).toBeDisabled();
        expect(editButtons[1]).not.toBeDisabled();
        expect(deleteButtons[1]).not.toBeDisabled();
    });

    it('shows the dependency indicator icon for values that are depended on', () => {
        const queryWithDeps: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
                { type: 'sum', code: 'virtual_2', operandCodes: ['virtual_1', '2020'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDeps);
        // The AccountTreeIcon has a title "computedValues.hasDependents" via Tooltip
        // Verify it renders by checking there is exactly one such tooltip target
        const indicator = screen.getByTestId('dependency-indicator');
        expect(indicator).toBeInTheDocument();
    });

    it('does not disable Edit and Delete buttons for a value with no virtual dependents', () => {
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDef);
        const editButton = screen.getByRole('button', { name: 'computedValues.edit' });
        const deleteButton = screen.getByRole('button', { name: 'computedValues.delete' });
        expect(editButton).not.toBeDisabled();
        expect(deleteButton).not.toBeDisabled();
    });
});

describe('ComputedValuesDialog — dialog title', () => {
    it('shows dialogTitle heading in list view', () => {
        renderDialog();
        expect(screen.getByRole('heading', { name: 'computedValues.dialogTitle' })).toBeInTheDocument();
    });

    it('shows addNew heading when adding a new computed value', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        expect(screen.getByRole('heading', { name: 'computedValues.addNew' })).toBeInTheDocument();
    });

    it('shows editTitle heading when editing an existing computed value', async () => {
        const user = userEvent.setup();
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderDialog(queryWithDef);
        await user.click(screen.getByRole('button', { name: 'computedValues.edit' }));
        expect(screen.getByRole('heading', { name: 'computedValues.editTitle' })).toBeInTheDocument();
    });
});

describe('ComputedValuesDialog — close behavior', () => {
    it('calls onClose when the Close button is clicked in list view', async () => {
        const user = userEvent.setup();
        const mockOnClose = jest.fn();
        renderDialog(baseDimensionQuery, jest.fn(), mockOnClose);
        await user.click(screen.getByRole('button', { name: 'computedValues.close' }));
        expect(mockOnClose).toHaveBeenCalledTimes(1);
    });
});

describe('ComputedValuesDialog — operator selection', () => {
    it('shows MultiplicationOperationForm when multiplication operator is selected', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorMultiplication' }));
        expect(screen.getByText('computedValues.multiplyByValue', { selector: 'label' })).toBeInTheDocument();
    });

    it('resets the operand form when the operator is changed', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorSubtraction' }));
        expect(screen.getByText('computedValues.subtractValue', { selector: 'label' })).toBeInTheDocument();
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorSum' }));
        expect(screen.queryByText('computedValues.subtractValue', { selector: 'label' })).not.toBeInTheDocument();
    });
});

describe('ComputedValuesDialog — validation', () => {
    it('shows validationMinOperands error when saving a sum with no operands selected', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.save' }));
        expect(screen.getByText('computedValues.validationMinOperands')).toBeInTheDocument();
    });

    it('shows validationSelectValue error when saving a subtraction with no operand selected', async () => {
        const user = userEvent.setup();
        renderDialog();
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.operatorSubtraction' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.save' }));
        expect(screen.getByText('computedValues.validationSelectValue')).toBeInTheDocument();
    });

    it('shows validationDivisionByZero error when editing a division with a constant of zero', async () => {
        const user = userEvent.setup();
        const divByZeroDef: IDivisionByConstantDefinition = {
            type: 'divisionByConstant',
            code: 'virtual_1',
            operand: '2018',
            constant: 0,
        };
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [divByZeroDef],
        };
        renderDialog(queryWithDef);
        await user.click(screen.getByRole('button', { name: 'computedValues.edit' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.save' }));
        expect(screen.getAllByText('computedValues.validationDivisionByZero').length).toBeGreaterThan(0);
    });
});

describe('ComputedValuesDialog — save', () => {
    it('calls onQueryChanged with the replaced definition and returns to list view when saving an edit', async () => {
        const user = userEvent.setup();
        const mockOnQueryChanged = jest.fn();
        const definition: ISumDefinition = {
            type: 'sum',
            code: 'virtual_1',
            operandCodes: ['2018', '2019'],
        };
        const queryWithDef: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [definition],
        };
        renderDialog(queryWithDef, mockOnQueryChanged);
        await user.click(screen.getByRole('button', { name: 'computedValues.edit' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.save' }));
        expect(mockOnQueryChanged).toHaveBeenCalledWith(
            expect.objectContaining({
                virtualValueDefinitions: [expect.objectContaining({ code: 'virtual_1', type: 'sum' })],
            })
        );
        expect(screen.getByRole('button', { name: 'computedValues.close' })).toBeInTheDocument();
    });

    it('calls onQueryChanged with the new definition and returns to list view when saving a new value', async () => {
        const user = userEvent.setup();
        const mockOnQueryChanged = jest.fn();
        renderDialog(baseDimensionQuery, mockOnQueryChanged);
        await user.click(screen.getByRole('button', { name: 'computedValues.addNew' }));
        const autocompleteInput = screen.getByLabelText('computedValues.selectValues');
        await user.click(autocompleteInput);
        await user.click(await screen.findByRole('option', { name: '2018' }));
        await user.click(autocompleteInput);
        await user.click(await screen.findByRole('option', { name: '2019' }));
        await user.click(screen.getByRole('button', { name: 'computedValues.save' }));
        expect(mockOnQueryChanged).toHaveBeenCalledWith(
            expect.objectContaining({
                virtualValueDefinitions: [expect.objectContaining({ type: 'sum' })],
            })
        );
        expect(screen.getByRole('button', { name: 'computedValues.close' })).toBeInTheDocument();
    });
});

describe('ComputedValuesDialog — operator symbol in list', () => {
    it('shows + for a sum definition with a constant', () => {
        const def: ISumDefinition = {
            type: 'sum',
            code: 'virtual_1',
            operandCodes: ['2018', '2019'],
            constant: 10,
        };
        renderDialog({ ...baseDimensionQuery, virtualValueDefinitions: [def] });
        expect(screen.getByText('2018, 2019 + 10')).toBeInTheDocument();
    });

    it('shows - for a subtraction-by-constant definition', () => {
        const def: ISubtractionByConstantDefinition = {
            type: 'subtractionByConstant',
            code: 'virtual_1',
            operand: '2018',
            constant: 5,
        };
        renderDialog({ ...baseDimensionQuery, virtualValueDefinitions: [def] });
        expect(screen.getByText('2018 - 5')).toBeInTheDocument();
    });

    it('shows × for a multiplication-by-constant definition', () => {
        const def: IMultiplicationByConstantDefinition = {
            type: 'multiplicationByConstant',
            code: 'virtual_1',
            operand: '2018',
            constant: 3,
        };
        renderDialog({ ...baseDimensionQuery, virtualValueDefinitions: [def] });
        expect(screen.getByText('2018 × 3')).toBeInTheDocument();
    });

    it('shows ÷ for a division-by-constant definition', () => {
        const def: IDivisionByConstantDefinition = {
            type: 'divisionByConstant',
            code: 'virtual_1',
            operand: '2018',
            constant: 50,
        };
        renderDialog({ ...baseDimensionQuery, virtualValueDefinitions: [def] });
        expect(screen.getByText('2018 ÷ 50')).toBeInTheDocument();
    });
});
