import React from 'react';
import { render } from "@testing-library/react";
import ManualPickVariableSelection from "./ManualPickVariableSelection";
import UiLanguageContext from "contexts/uiLanguageContext";

const mockVariableValues = [
    {
        code: "2018",
        name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2019",
        name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2020",
        name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2021",
        name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        note: null,
        isSum: false,
        contentComponent: null
    }
];

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

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
    it('renders correctly', () => {
        const { asFragment } = render(
        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
            <ManualPickVariableSelection
                options={mockVariableValues}
                selectedValues={[mockVariableValues[0], mockVariableValues[1]]}
                onQueryChanged={() => {}}/>
        </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});