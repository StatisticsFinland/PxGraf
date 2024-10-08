import React from 'react';
import { render } from "@testing-library/react";
import { IVariable, VariableType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import EditorFilterSection from "./EditorFilterSection";
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockVariables: IVariable[] = [
    {
        code: 'foo',
        name: {
            'fi': 'name'
        },
        note: {
            'fi': 'note'
        },
        type: VariableType.Content,
        values: [
            {
                code: 'bar',
                isSum: false,
                name: {
                    'fi': 'name'
                },
                note: {
                    'fi': 'note'
                }
            }
        ]
    }
]
const mockQuery: Query = {
    'foo': {
        virtualValueDefinitions: [],
        selectable: true,
        valueFilter: {
            type: FilterType.All,
            query: 'bar'
        }
    }
}

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => {}),
            },
        };
    },
}));

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
                <EditorFilterSection
                    queries={mockQuery}
                    resolvedVariableCodes={{ 'foo': ['foo', 'bar', 'baz'] }}
                    variables={mockVariables}
                    />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});