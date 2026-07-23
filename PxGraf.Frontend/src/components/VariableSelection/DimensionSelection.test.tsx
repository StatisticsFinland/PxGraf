import React from 'react';
import { render, screen, within } from "@testing-library/react";
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import DimensionSelection from "./DimensionSelection";
import { areDimensionSelectionPropsEqual } from "./dimensionSelectionProps";
import UiLanguageContext from "contexts/uiLanguageContext";
import { QueryContext } from "contexts/queryContext";

const mockDimension: IDimension =
{
    code: "Vuosi",
    name: {
        fi: "Vuosi",
        sv: "År",
        en: "Year"
    },
    type: EDimensionType.Time,
    values: [
        {
            code: "2018",
            name: {
                fi: "2018",
                sv: "2018",
                en: "2018"
            },
            isVirtual: false
        },
        {
            code: "2019",
            name: {
                fi: "2019",
                sv: "2019",
                en: "2019"
            },
            isVirtual: false
        },
        {
            code: "2020",
            name: {
                fi: "2020",
                sv: "2020",
                en: "2020"
            },
            isVirtual: false
        },
        {
            code: "2021",
            name: {
                fi: "2021*",
                sv: "2021*",
                en: "2021*"
            },
            isVirtual: false
        }
    ]
}

const mockQuery: Query = {
    Vuosi: {
        valueFilter: {
            type: FilterType.Top,
            query: 4
        },
        selectable: false,
        virtualValueDefinitions: []
    }
};

describe('Render stability', () => {
    it('treats equivalent resolved-code arrays and unrelated query updates as equal', () => {
        const previousProps = {
            dimension: mockDimension,
            resolvedDimensionValueCodes: ["2018", "2019"],
            query: mockQuery,
        };
        const nextProps = {
            ...previousProps,
            resolvedDimensionValueCodes: ["2018", "2019"],
            query: {
                ...mockQuery,
                Other: {
                    valueFilter: { type: FilterType.All },
                    selectable: false,
                    virtualValueDefinitions: [],
                }
            },
        };

        expect(areDimensionSelectionPropsEqual(previousProps, nextProps)).toBe(true);
    });

    it('rerenders when this dimension resolved codes change', () => {
        const previousProps = {
            dimension: mockDimension,
            resolvedDimensionValueCodes: ["2018", "2019"],
            query: mockQuery,
        };
        const nextProps = {
            ...previousProps,
            resolvedDimensionValueCodes: ["2018", "2020"],
        };

        expect(areDimensionSelectionPropsEqual(previousProps, nextProps)).toBe(false);
    });
});

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
            <DimensionSelection
                dimension={mockDimension}
                resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                query={mockQuery}/>
        </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Stale virtual code regression', () => {
    it('does not crash and omits stale codes when FilterType.Item query references a missing value code', () => {
        const dimensionWithOnlyReal: IDimension = {
            ...mockDimension,
            values: [
                {
                    code: '2018',
                    name: { fi: '2018', sv: '2018', en: '2018' },
                    isVirtual: false,
                },
            ],
        };
        const queryWithStaleCode: Query = {
            Vuosi: {
                valueFilter: {
                    type: FilterType.Item,
                    query: ['2018', 'stale_virtual_code'],
                },
                selectable: false,
                virtualValueDefinitions: [],
            },
        };
        expect(() =>
            render(
                <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                    <DimensionSelection
                        dimension={dimensionWithOnlyReal}
                        resolvedDimensionValueCodes={['2018']}
                        query={queryWithStaleCode}
                    />
                </UiLanguageContext.Provider>
            )
        ).not.toThrow();

        // '2018' chip is rendered; 'stale_virtual_code' chip is not
        expect(screen.getByText('2018')).toBeInTheDocument();
        expect(screen.queryByText('stale_virtual_code')).not.toBeInTheDocument();
    });
});

describe('Filter method selector', () => {
    it('renders the current filter method and updates the query when a new method is chosen', async () => {
        const user = userEvent.setup();
        const setQuery = jest.fn();

        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{ cubeQuery: { variableQueries: {} }, setCubeQuery: jest.fn(), query: mockQuery, setQuery }}>
                    <DimensionSelection
                        dimension={mockDimension}
                        resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                        query={mockQuery}
                    />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );

        const filterMethodSelect = screen.getByRole('combobox', { name: 'variableSelect.filterMethodLabel' });
        expect(within(filterMethodSelect).getByText('variableSelect.topFilter')).toBeInTheDocument();

        await user.click(filterMethodSelect);
        const allOption = await screen.findByRole('option', { name: 'variableSelect.allFilter' });
        await user.click(allOption);

        const updateQuery = setQuery.mock.calls[0][0];
        expect(updateQuery(mockQuery)).toEqual({
            Vuosi: {
                valueFilter: { type: FilterType.All },
                selectable: false,
                virtualValueDefinitions: []
            }
        });
    });
});

describe('Computed values action', () => {
    it('renders as an accessible icon action that opens the dialog', async () => {
        const user = userEvent.setup();

        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <DimensionSelection
                    dimension={mockDimension}
                    resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                    query={mockQuery}
                />
            </UiLanguageContext.Provider>
        );

        const computedValuesAction = screen.getByRole('button', { name: 'computedValues.button' });
        expect(screen.queryByText('computedValues.dialogTitle')).not.toBeInTheDocument();

        await user.click(computedValuesAction);
        expect(screen.getByText('computedValues.dialogTitle')).toBeInTheDocument();
    });
});

describe('Selectable mode', () => {
    it('keeps selectable controls out of the layout while settings are closed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <DimensionSelection
                    dimension={mockDimension}
                    resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                    query={mockQuery}
                />
            </UiLanguageContext.Provider>
        );

        const settingsButton = screen.getByRole('button', { name: 'variableSelect.selectableSettings' });
        expect(settingsButton).toHaveAttribute('aria-expanded', 'false');
        expect(settingsButton).not.toHaveAttribute('aria-pressed');
        expect(screen.queryByTestId('selectability-active-marker')).not.toBeInTheDocument();
        expect(screen.queryByRole('switch', { name: 'variableSelect.selectable' })).not.toBeInTheDocument();
        expect(screen.queryByLabelText('selectable.labelText')).not.toBeInTheDocument();
    });

    it('shows the active state and default value field when settings are opened', async () => {
        const user = userEvent.setup();
        const selectableQuery: Query = {
            Vuosi: {
                ...mockQuery.Vuosi,
                selectable: true
            }
        };

        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <DimensionSelection
                    dimension={mockDimension}
                    resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                    query={selectableQuery}
                />
            </UiLanguageContext.Provider>
        );

        const settingsButton = screen.getByRole('button', { name: 'variableSelect.selectableSettingsActive' });
        expect(screen.getByTestId('selectability-active-marker')).toBeInTheDocument();
        await user.hover(settingsButton);
        expect(await screen.findByRole('tooltip')).toHaveTextContent('variableSelect.selectableSettingsActive');

        await user.click(settingsButton);

        expect(screen.getByRole('dialog', { name: 'variableSelect.selectableSettings' })).toBeInTheDocument();
        const selectabilitySwitch = screen.getByRole('switch', { name: 'variableSelect.selectable' });
        expect(selectabilitySwitch).toBeChecked();
        expect(selectabilitySwitch).toHaveFocus();
        expect(screen.getByLabelText('selectable.labelText')).toBeInTheDocument();
    });

    it('updates the query when the switch is toggled', async () => {
        const user = userEvent.setup();
        const setQuery = jest.fn();

        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{ cubeQuery: { variableQueries: {} }, setCubeQuery: jest.fn(), query: mockQuery, setQuery }}>
                    <DimensionSelection
                        dimension={mockDimension}
                        resolvedDimensionValueCodes={["2018", "2019", "2020", "2021*"]}
                        query={mockQuery}
                    />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );

        await user.click(screen.getByRole('button', { name: 'variableSelect.selectableSettings' }));
        await user.click(screen.getByRole('switch', { name: 'variableSelect.selectable' }));

        const updateQuery = setQuery.mock.calls[0][0];
        expect(updateQuery(mockQuery)).toEqual({
            Vuosi: {
                valueFilter: { type: FilterType.Top, query: 4 },
                selectable: true,
                virtualValueDefinitions: []
            }
        });
    });
});