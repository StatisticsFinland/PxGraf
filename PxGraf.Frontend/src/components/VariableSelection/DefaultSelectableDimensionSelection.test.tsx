import React from 'react';
import { render } from "@testing-library/react";
import { IDimensionValue } from "../../types/cubeMeta";
import DefaultSelectableDimensionSelection from "./DefaultSelectableDimensionSelection";
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { EditorContext } from 'contexts/editorContext';

const mockDimensionValues: IDimensionValue[] = [
    {
        code: "2018",
        name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        isVirtual: false,
    },
    {
        code: "2019",
        name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        isVirtual: false,
    },
    {
        code: "2020",
        name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        isVirtual: false,
    },
    {
        code: "2021",
        name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        isVirtual: false,
    }
];

const mockResolvedCodes: string[] = ['2018', '2019'];

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const defaultSelectables = { foo: ['2018'] };
const setDefaultSelectables = jest.fn();
const cubeQuery = null;
const setCubeQuery = jest.fn();
const query = null;
const setQuery = jest.fn();
const saveDialogOpen = false;
const setSaveDialogOpen = jest.fn();
const selectedVisualizationUserInput = null;
const setSelectedVisualizationUserInput = jest.fn();
const visualizationSettingsUserInput = null;
const setVisualizationSettingsUserInput = jest.fn();

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

describe('Rendering test', () => {
    it('renders correctly with default selectable', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{ cubeQuery, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables, loadedQueryId: '', setLoadedQueryId: jest.fn(), loadedQueryIsDraft: false, setLoadedQueryIsDraft: jest.fn(), publicationEnabled: true, setPublicationEnabled: jest.fn() }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={mockResolvedCodes}
                        dimensionCode={'foo'}
                    />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly without default selectable', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{ cubeQuery, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables: {}, setDefaultSelectables, loadedQueryId: '', setLoadedQueryId: jest.fn(), loadedQueryIsDraft: false, setLoadedQueryIsDraft: jest.fn(), publicationEnabled: true, setPublicationEnabled: jest.fn() }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={mockResolvedCodes}
                        dimensionCode={'foo'}
                    />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly without resolved dimension value codes', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorContext.Provider value={{ cubeQuery, setCubeQuery, query, setQuery, saveDialogOpen, setSaveDialogOpen, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput, defaultSelectables, setDefaultSelectables, loadedQueryId: '', setLoadedQueryId: jest.fn(), loadedQueryIsDraft: false, setLoadedQueryIsDraft: jest.fn(), publicationEnabled: true, setPublicationEnabled: jest.fn() }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={[]}
                        dimensionCode={'foo'}
                    />
                </EditorContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });
});