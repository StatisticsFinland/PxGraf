import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { ComputedValuesButton } from './ComputedValuesButton';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { FilterType, IDimensionQuery, ISumDefinition } from 'types/query';

const mockDimension: IDimension = {
    code: 'Vuosi',
    name: { en: 'Year', fi: 'Vuosi', sv: 'År' },
    type: EDimensionType.Time,
    values: [
        { code: '2018', name: { en: '2018', fi: '2018', sv: '2018' }, isVirtual: false },
        { code: '2019', name: { en: '2019', fi: '2019', sv: '2019' }, isVirtual: false },
    ],
};

const baseDimensionQuery: IDimensionQuery = {
    valueFilter: { type: FilterType.Top, query: 2 },
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

const renderButton = (dimensionQuery = baseDimensionQuery, onQueryChanged = jest.fn()) =>
    render(
        <UiLanguageContext.Provider value={uiLanguageContextValue}>
            <ComputedValuesButton
                dimension={mockDimension}
                dimensionQuery={dimensionQuery}
                onQueryChanged={onQueryChanged}
            />
        </UiLanguageContext.Provider>
    );

describe('ComputedValuesButton', () => {
    it('renders button with label', () => {
        renderButton();
        const button = screen.getByRole('button', { name: 'computedValues.button' });
        expect(button).toBeInTheDocument();
        expect(button).not.toHaveClass('MuiIconButton-colorPrimary');
    });

    it('shows badge count when definitions are present', () => {
        const queryWithDefs: IDimensionQuery = {
            ...baseDimensionQuery,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
                { type: 'sum', code: 'virtual_2', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderButton(queryWithDefs);
        expect(screen.getByText('2')).toBeInTheDocument();
        expect(screen.getByRole('button', { name: 'computedValues.buttonWithCount' })).toHaveClass('MuiIconButton-colorPrimary');
    });

    it('opens dialog on button click', async () => {
        const user = userEvent.setup();
        renderButton();
        await user.click(screen.getByRole('button', { name: 'computedValues.button' }));
        expect(screen.getByText('computedValues.dialogTitle')).toBeInTheDocument();
    });

    it('does not show dialog before button is clicked', () => {
        renderButton();
        expect(screen.queryByText('computedValues.dialogTitle')).not.toBeInTheDocument();
    });

    it('removes the deleted virtual code from a FilterType.Item query when onQueryChanged is called', async () => {
        const user = userEvent.setup();
        const mockOnQueryChanged = jest.fn();
        const queryWithVirtualSelected: IDimensionQuery = {
            valueFilter: { type: FilterType.Item, query: ['2018', 'virtual_1'] },
            selectable: false,
            virtualValueDefinitions: [
                { type: 'sum', code: 'virtual_1', operandCodes: ['2018', '2019'] } as ISumDefinition,
            ],
        };
        renderButton(queryWithVirtualSelected, mockOnQueryChanged);
        await user.click(screen.getByRole('button', { name: 'computedValues.buttonWithCount' }));
        await user.click(screen.getByRole('button', { name: /^computedValues\.delete/ }));
        expect(mockOnQueryChanged).toHaveBeenCalledTimes(1);
        const calledWith: IDimensionQuery = mockOnQueryChanged.mock.calls[0][0];
        expect(calledWith.virtualValueDefinitions).toEqual([]);
        expect((calledWith.valueFilter.query as string[])).toContain('2018');
        expect((calledWith.valueFilter.query as string[])).not.toContain('virtual_1');
    });
});

