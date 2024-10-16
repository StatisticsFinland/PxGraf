import React from 'react';
import { render } from "@testing-library/react";
import ManualPickVariableSelection from "./ManualPickVariableSelection";
import UiLanguageContext from "contexts/uiLanguageContext";
import { IDimensionValue } from "../../../types/cubeMeta";

const mockVariableValues: IDimensionValue[] = [
    {
        Code: "2018",
        Name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        IsVirtual: false,
    },
    {
        Code: "2019",
        Name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        IsVirtual: false,
    },
    {
        Code: "2020",
        Name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        IsVirtual: false,
    },
    {
        Code: "2021",
        Name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        IsVirtual: false,
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