import React from 'react';
import { render } from "@testing-library/react";
import { IDimensionValue } from "../../types/cubeMeta";
import DefaultSelectableDimensionSelection from "./DefaultSelectableDimensionSelection";
import UiLanguageContext from '../../contexts/uiLanguageContext';
import { VisualizationContext } from 'contexts/visualizationContext';

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
const selectedVisualizationUserInput = null;
const setSelectedVisualizationUserInput = jest.fn();
const visualizationSettingsUserInput = null;
const setVisualizationSettingsUserInput = jest.fn();

describe('Rendering test', () => {
    it('renders correctly with default selectable', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <VisualizationContext.Provider value={{ defaultSelectables, setDefaultSelectables, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={mockResolvedCodes}
                        dimensionCode={'foo'}
                    />
                </VisualizationContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly without default selectable', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <VisualizationContext.Provider value={{ defaultSelectables: {}, setDefaultSelectables, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={mockResolvedCodes}
                        dimensionCode={'foo'}
                    />
                </VisualizationContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly without resolved dimension value codes', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <VisualizationContext.Provider value={{ defaultSelectables, setDefaultSelectables, selectedVisualizationUserInput, setSelectedVisualizationUserInput, visualizationSettingsUserInput, setVisualizationSettingsUserInput }}>
                    <DefaultSelectableDimensionSelection
                        options={mockDimensionValues}
                        resolvedDimensionValueCodes={[]}
                        dimensionCode={'foo'}
                    />
                </VisualizationContext.Provider>
            </UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });
});