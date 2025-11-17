import { render } from '@testing-library/react';
import { ISortingOption } from "../../../types/editorContentsResponse";
import { IVisualizationSettings } from "../../../types/visualizationSettings";
import UiLanguageContext from '../../../contexts/uiLanguageContext';
import { EditorContext } from '../../../contexts/editorContext';
import React from 'react';
import SortingSelector from './SortingSelector';
import userEvent from '@testing-library/user-event';

const visualizationSettings: IVisualizationSettings = {
    sorting: "foo"
};

const sortingOptions: ISortingOption[] = [
    {
        code: "foo",
        description: {
            fi: "foo.fi",
            sv: "foo.sv",
            en: "foo.en"
        }
    },
    {
        code: "bar",
        description: {
            fi: "bar.fi",
            sv: "bar.sv",
            en: "bar.en"
        }
    }
];

const languageContext =
{
    language: 'fi',
    setLanguage: jest.fn(),
    languageTab: 'fi',
    setLanguageTab: jest.fn(),
    availableUiLanguages: ['fi', 'en', 'sv'],
    uiContentLanguage: 'fi',
    setUiContentLanguage: jest.fn()
};

const editorContext = {
    cubeQuery: null,
    setCubeQuery: jest.fn(),
    query: null,
    setQuery: jest.fn(),
    saveDialogOpen: false,
    setSaveDialogOpen: jest.fn(),
    selectedVisualizationUserInput: null,
    setSelectedVisualizationUserInput: jest.fn(),
    visualizationSettingsUserInput: null,
    setVisualizationSettingsUserInput: jest.fn(),
    defaultSelectables: null,
    setDefaultSelectables: jest.fn(),
    loadedQueryId: '',
    setLoadedQueryId: jest.fn(),
    loadedQueryIsDraft: false,
    setLoadedQueryIsDraft: jest.fn(),
    publicationWebhookEnabled: true,
    setPublicationWebhookEnabled: jest.fn()
};

describe('rendering test', () => {
    it('renders sorting selector correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={languageContext}>
                <EditorContext.Provider value={editorContext}>
                    <SortingSelector visualizationSettings={visualizationSettings} sortingOptions={sortingOptions} />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );

        expect(asFragment()).toMatchSnapshot();
    })
})

describe('functionality test', () => {
    it('calls setVisualizationSettingsUserInput with the correct value when value is selected', async () => {
        const user = userEvent.setup();
        const mockSetVisualizationSettingsUserInput = jest.fn();
        const { getByLabelText, getByText } = render(
            <UiLanguageContext.Provider value={languageContext}>
                <EditorContext.Provider value={{ ...editorContext, setVisualizationSettingsUserInput: mockSetVisualizationSettingsUserInput }}>
                    <SortingSelector visualizationSettings={visualizationSettings} sortingOptions={sortingOptions} />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>
        );
        const sortingSelector = getByLabelText('chartSettings.sort');
        await user.click(sortingSelector);
        const option = getByText('bar.fi');
        await user.click(option);
        expect(mockSetVisualizationSettingsUserInput).toHaveBeenCalledWith({
            ...visualizationSettings,
            sorting: 'bar'
        });
    });
});