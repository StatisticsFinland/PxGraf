import React from 'react';
import { render } from "@testing-library/react";
import { IDimension, EDimensionType } from "types/cubeMeta";
import { FilterType, Query } from "types/query";
import EditorFilterSection from "./EditorFilterSection";
import UiLanguageContext from 'contexts/uiLanguageContext';

const mockDimensions: IDimension[] = [
    {
        code: 'foo',
        name: {
            'fi': 'name'
        },
        type: EDimensionType.Content,
        values: [
            {
                code: 'bar',
                name: {
                    'fi': 'name'
                },
                isVirtual: false
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
                    resolvedDimensionCodes={{ 'foo': ['foo', 'bar', 'baz'] }}
                    dimensions={mockDimensions}
                    />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});