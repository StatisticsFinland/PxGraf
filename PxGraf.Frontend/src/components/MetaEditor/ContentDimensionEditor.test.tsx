import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { EMetaPropertyType, IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery, IDimensionEditions } from 'types/query';
import { ContentDimensionEditor } from './ContentDimensionEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { QueryContext } from '../../contexts/queryContext';

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

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
            isVirtual: false,
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
const mockFunction = jest.fn();
const mockDimensionEdits: IDimensionEditions = {
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
        foo: mockDimensionEdits
    }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{
                    cubeQuery: mockCubeQuery,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                }}>
                    <ContentDimensionEditor language={mockLang} dimension={mockDimension} />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

const renderEditor = (
    dimension: IDimension = mockDimension,
    cubeQuery: ICubeQuery = mockCubeQuery,
    setCubeQuery = jest.fn(),
) =>
    render(
        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
            <QueryContext.Provider value={{ cubeQuery, setCubeQuery, query: {}, setQuery: jest.fn() }}>
                <ContentDimensionEditor language={mockLang} dimension={dimension} />
            </QueryContext.Provider>
        </UiLanguageContext.Provider>
    );

describe('Layout tests', () => {
    it('renders name, unit and source editor fields for each dimension value', () => {
        renderEditor();
        expect(screen.getByDisplayValue('bar')).toBeInTheDocument();     // name (edit value)
        expect(screen.getByDisplayValue('yksikko')).toBeInTheDocument(); // unit (default)
        expect(screen.getByDisplayValue('lahde')).toBeInTheDocument();   // source (default)
    });

    it('renders editors for all values in a multi-value dimension', () => {
        const multiDimension: IDimension = {
            ...mockDimension,
            values: [
                {
                    code: 'bar',
                    name: { fi: 'fgfgfg', sv: 'fgfgfg', en: 'fgfgfg' },
                    isVirtual: false,
                    unit: { fi: 'unit1', sv: 'unit1', en: 'unit1' },
                    precision: 0,
                    lastUpdated: '2021-01-01',
                    additionalProperties: {
                        SOURCE: {
                            type: EMetaPropertyType.MultilanguageText,
                            value: { fi: 'source1', sv: 'source1', en: 'source1' }
                        }
                    }
                },
                {
                    code: 'baz',
                    name: { fi: 'second', sv: 'second', en: 'second' },
                    isVirtual: false,
                    unit: { fi: 'unit2', sv: 'unit2', en: 'unit2' },
                    precision: 0,
                    lastUpdated: '2021-01-01',
                    additionalProperties: {
                        SOURCE: {
                            type: EMetaPropertyType.MultilanguageText,
                            value: { fi: 'source2', sv: 'source2', en: 'source2' }
                        }
                    }
                }
            ]
        };
        renderEditor(multiDimension, { chartHeaderEdit: {}, variableQueries: {} });
        expect(screen.getByDisplayValue('unit1')).toBeInTheDocument();
        expect(screen.getByDisplayValue('unit2')).toBeInTheDocument();
    });
});

describe('Assertion tests', () => {
    it('calls setCubeQuery with the updated name edit when name is changed', () => {
        const setCubeQuery = jest.fn();
        renderEditor(mockDimension, mockCubeQuery, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'newName' } });
        expect(setCubeQuery).toHaveBeenCalledWith({
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: {
                            nameEdit: { fi: 'newName', sv: 'bar', en: 'bar' }
                        }
                    }
                }
            }
        });
    });

    it('calls setCubeQuery with the updated unit edit when unit is changed', () => {
        const setCubeQuery = jest.fn();
        renderEditor(mockDimension, mockCubeQuery, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('yksikko'), { target: { value: 'newUnit' } });
        expect(setCubeQuery).toHaveBeenCalledWith({
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: {
                            nameEdit: { fi: 'bar', sv: 'bar', en: 'bar' },
                            contentComponent: { unitEdit: { fi: 'newUnit' } }
                        }
                    }
                }
            }
        });
    });

    it('calls setCubeQuery with the updated source edit when source is changed', () => {
        const setCubeQuery = jest.fn();
        renderEditor(mockDimension, mockCubeQuery, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('lahde'), { target: { value: 'newSource' } });
        expect(setCubeQuery).toHaveBeenCalledWith({
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: {
                            nameEdit: { fi: 'bar', sv: 'bar', en: 'bar' },
                            contentComponent: { sourceEdit: { fi: 'newSource' } }
                        }
                    }
                }
            }
        });
    });

    it('creates the correct structure when there are no pre-existing dimension edits', () => {
        const setCubeQuery = jest.fn();
        const emptyQuery: ICubeQuery = { chartHeaderEdit: {}, variableQueries: {} };
        renderEditor(mockDimension, emptyQuery, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('fgfgfg'), { target: { value: 'newName' } });
        expect(setCubeQuery).toHaveBeenCalledWith({
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: {
                            nameEdit: { fi: 'newName' }
                        }
                    }
                }
            }
        });
    });
});