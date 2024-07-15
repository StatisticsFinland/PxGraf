import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { TablePivotSettings } from "./TablePivotSettings";
import { VariableType } from "types/cubeMeta";
import UiLanguageContext from 'contexts/uiLanguageContext';
import { Query, FilterType } from "types/query";
import '@testing-library/jest-dom';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
}));

const mockVisualizationRules = {
    allowManualPivot: false,
    sortingOptions: null,
    multiselectVariableAllowed: false
};

const mockVisualizationSettings = {
    defaultSelectableVariableCodes: null,
    pivotRequested: null,
    cutYAxis: null,
    multiselectableVariableCode: null,
    rowVariableCodes: ["foobar1", "foobar3"],
    columnVariableCodes: ["foobar2", "foobar4"],
    sorting: null,
    matchXLabelsToEnd: false,
    markerSize: null,
}

const mockVariables = [
    {
        code: "foobar1",
        name: { fi: "foo1", sv: "bar1", en: "foobar1" },
        note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
        type: VariableType.OtherClassificatory,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false
            }
        ]
    },
    {
        code: "foobar2",
        name: { fi: "foo2", sv: "bar2", en: "foobar2" },
        note: { fi: "föö2", sv: "bär2", en: "fööbär2" },
        type: VariableType.OtherClassificatory,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false
            }
        ]
    },
    {
        code: "foobar3",
        name: { fi: "foo3", sv: "bar3", en: "foobar3" },
        note: { fi: "föö3", sv: "bär3", en: "fööbär3" },
        type: VariableType.OtherClassificatory,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false
            }
        ]
    },
    {
        code: "foobar4",
        name: { fi: "foo4", sv: "bar4", en: "foobar4" },
        note: { fi: "föö4", sv: "bär4", en: "fööbär4" },
        type: VariableType.OtherClassificatory,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false
            }
        ]
    },
    {
        code: "foobar5",
        name: { fi: "foo5", sv: "bar5", en: "foobar5" },
        note: { fi: "föö5", sv: "bär5", en: "fööbär5" },
        type: VariableType.OtherClassificatory,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            }
        ]
    }
]

const mockQuery: Query = {
    foobar1: {
        valueFilter: {
            type: FilterType.Item,
            query: 2
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    foobar2: {
        valueFilter: {
            type: FilterType.Item,
            query: 2
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    foobar3: {
        valueFilter: {
            type: FilterType.Item,
            query: 2
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    foobar4: {
        valueFilter: {
            type: FilterType.Item,
            query: 2
        },
        selectable: false,
        virtualValueDefinitions: null
    },
    foobar5: {
        valueFilter: {
            type: FilterType.Item,
            query: 1
        },
        selectable: false,
        virtualValueDefinitions: null
    }
};

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const mockSettingsChangedHandler = jest.fn();
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('table settings component should have 3 buttons, up, down and switch', () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getAllByRole('button').length).toBe(7); // up, down, switch and 4 variables === 7
    });

    it('Should call settingschangedhandler when moving row up', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo3 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[0]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Should call settingschangedhandler when moving col up', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo4 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[0]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Should call settingschangedhandler when moving row down', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo1 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[1]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Should call settingschangedhandler when moving col down', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo2 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[1]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Should call settingschangedhandler when moving row', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo3 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[2]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Should call settingschangedhandler when moving column', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('foo4 2'));
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[2]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).toBeCalled();
        });
    });

    it('Shouldnt call settingschangedhandler if nothing selected', async () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        fireEvent.click(screen.getAllByText((_content, element) => element.tagName.toLowerCase() === 'button')[0]);
        await waitFor(() => {
            expect(mockSettingsChangedHandler).not.toBeCalled();
        });
    });

    it('When a column variable code is provided in the settings parameter a corresponding list items should be rendered in the column variables list', () => {
        const mockSettingsChangedHandler = jest.fn();
        const toggledMockSettings = { ...mockVisualizationSettings, cutYAxis: true }
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={toggledMockSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.columnVariables').parentElement).toContainElement(screen.getByText("foo2 2"));
    });

    it('When a row variable code is provided in the settings parameter a corresponding list items should be rendered in the row variables list', () => {
        const mockSettingsChangedHandler = jest.fn();
        const toggledMockSettings = { ...mockVisualizationSettings, cutYAxis: true }
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={toggledMockSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.rowVariables').parentElement).toContainElement(screen.getByText("foo1 2"));
    });

    it('When a table with single value variables is created, single value variables should not show up in pivot settings', () => {
        const mockSettingsChangedHandler = jest.fn();
        const toggledMockSettings = { ...mockVisualizationSettings, columnVariableCodes: ["foobar2", "foobar5"] }
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={toggledMockSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.columnVariables').parentElement).toHaveTextContent("foo2 2");
        expect(screen.getByText('chartSettings.columnVariables').parentElement).not.toHaveTextContent("foo5 1");
    });

    it('When a table with a selectable multivalue variable is created, said variable should not show up in pivot settings', () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    selectableVariables={[mockVariables[1]]}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.columnVariables').parentElement).not.toHaveTextContent("foo2 2");
    });

    it('When a table with a single value variable with a filter type "from" or "all" is created, said variable should show up in the pivot settings', () => {
        const mockSettingsChangedHandler = jest.fn();
        const toggledMockSettings = { ...mockVisualizationSettings, columnVariableCodes: ["foobar2", "foobar5"] }
        const mockQueryWithPotentiallyGrowingVariable = { ...mockQuery, foobar5: { ...mockQuery.foobar5, valueFilter: { type: FilterType.All } } }
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={toggledMockSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQueryWithPotentiallyGrowingVariable}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.columnVariables').parentElement).toHaveTextContent("foo2 2");
        expect(screen.getByText('chartSettings.columnVariables').parentElement).toHaveTextContent("foo5 1");
    });

    it('When a table with with a selectable multivalue variable that is set as the multiselectable variable is created, said variable should show up in the pivot settings', () => {
        const mockSettingsChangedHandler = jest.fn();
        const multiselectableMockSettings = { ...mockVisualizationSettings, multiselectableVariableCode: "foobar2" };
        const multiselectableVisualizatioRules = { ...mockVisualizationRules, multiselectVariableAllowed: true };
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <TablePivotSettings
                    variables={mockVariables}
                    visualizationRules={multiselectableVisualizatioRules}
                    visualizationSettings={multiselectableMockSettings}
                    settingsChangedHandler={mockSettingsChangedHandler}
                    query={mockQuery}
                ></TablePivotSettings>
            </UiLanguageContext.Provider>);
        expect(screen.getByText('chartSettings.columnVariables').parentElement).toHaveTextContent("foo2 2");
    });
});