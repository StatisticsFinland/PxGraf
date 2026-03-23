import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { EMetaPropertyType, IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery, IDimensionEditions } from 'types/query';
import DimensionEditor from './DimensionEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { QueryContext } from '../../contexts/queryContext';

const mockDimension: IDimension = {
    code: 'foo',
    name: {
        'fi': 'asd',
        'sv': 'asd',
        'en': 'asd'
    },
    type: EDimensionType.Content,
    values: [
        {
            code: 'bar',
            name: {
                'fi': 'fgfgfg',
                'sv': 'fgfgfg',
                'en': 'fgfgfg'
            },
            isVirtual: true,
            unit: {
                'fi': 'yksikko',
                'sv': 'enhet',
                'en': 'unit'
            },
            precision: 0,
            lastUpdated: '2021-01-01',
            additionalProperties: {
                SOURCE: {
                    type: EMetaPropertyType.MultilanguageText,
                    value: {
                        'fi': 'lahde',
                        'sv': 'kalla',
                        'en': 'source'
                    }
                }
            }
        }
    ]
}
const mockLang = 'fi';
const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();
const mockDimEdits: IDimensionEditions = {
    valueEdits: {
        'bar': {
            nameEdit: {
                'fi': 'bar',
                'sv': 'bar',
                'en': 'bar'
            }
        }
    }
};
const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: {},
    variableQueries: {
        'foo': mockDimEdits
    }
}

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{
                    cubeQuery: mockCubeQuery,
                    setCubeQuery: jest.fn(),
                    query: {},
                    setQuery: jest.fn(),
                }}>
                    <DimensionEditor language={mockLang} dimension={mockDimension} />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});
