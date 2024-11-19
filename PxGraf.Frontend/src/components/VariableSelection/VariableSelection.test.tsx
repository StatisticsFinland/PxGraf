import React from 'react';
import { render } from "@testing-library/react";
import { IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, IVariableQuery } from "types/query";
import VariableSelection from "./VariableSelection";
import UiLanguageContext from "contexts/uiLanguageContext";

const mockVariable: IDimension =
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

const mockQuery: IVariableQuery = {
    valueFilter: {
        type: FilterType.Top,
        query: 4
    },
    selectable: false,
    virtualValueDefinitions: null
};

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
            <VariableSelection
                variable={mockVariable}
                resolvedVariableValueCodes={["2018", "2019", "2020", "2021*"]}
                query={mockQuery}
                onQueryChanged={(newValues) => {}}/>
        </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});