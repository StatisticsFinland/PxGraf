import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import BasicDimensionEditor from './BasicDimensionEditor';
import '@testing-library/jest-dom';
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
            isVirtual: false
        }
    ]
}

const mockLang = 'fi';
const mockFunction = jest.fn();

const mockDimEdits: ICubeQuery = {
    chartHeaderEdit: {},
    variableQueries: {
        foo: {
            valueEdits: {
                'bar': {
                    nameEdit: {
                        'fi': 'bar',
                        'sv': 'bar',
                        'en': 'bar'
                    }
                }
            }
        }
    }
};

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{
                    cubeQuery: mockDimEdits,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                }}>
                    <BasicDimensionEditor language={mockLang} dimension={mockDimension} />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{
                    cubeQuery: mockDimEdits,
                    setCubeQuery: mockFunction,
                    query: {},
                    setQuery: jest.fn(),
                }}>
                    <BasicDimensionEditor language={mockLang} dimension={mockDimension} />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'editValue2' } });
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});

const renderEditor = (
    dimension: IDimension = mockDimension,
    cubeQuery: ICubeQuery = mockDimEdits,
    setCubeQuery = jest.fn(),
) =>
    render(
        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
            <QueryContext.Provider value={{ cubeQuery, setCubeQuery, query: {}, setQuery: jest.fn() }}>
                <BasicDimensionEditor language={mockLang} dimension={dimension} />
            </QueryContext.Provider>
        </UiLanguageContext.Provider>
    );

const resolveCubeQueryUpdate = (setCubeQuery: jest.Mock, currentCubeQuery: ICubeQuery) => {
    const update = setCubeQuery.mock.calls[0][0] as React.SetStateAction<ICubeQuery>;
    return typeof update === 'function' ? update(currentCubeQuery) : update;
};

describe('Layout tests', () => {
    it('renders an editor field for the dimension value', () => {
        renderEditor();
        expect(screen.getByDisplayValue('bar')).toBeInTheDocument();
    });

    it('renders editor fields for all values in a multi-value dimension', () => {
        const multiDimension: IDimension = {
            ...mockDimension,
            values: [
                { code: 'bar', name: { fi: 'fgfgfg', sv: 'fgfgfg', en: 'fgfgfg' }, isVirtual: false },
                { code: 'baz', name: { fi: 'second', sv: 'second', en: 'second' }, isVirtual: false },
            ]
        };
        const multiCubeQuery: ICubeQuery = {
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: { nameEdit: { fi: 'barEdit', sv: 'barEdit', en: 'barEdit' } },
                        baz: { nameEdit: { fi: 'bazEdit', sv: 'bazEdit', en: 'bazEdit' } },
                    }
                }
            }
        };
        renderEditor(multiDimension, multiCubeQuery);
        expect(screen.getByDisplayValue('barEdit')).toBeInTheDocument();
        expect(screen.getByDisplayValue('bazEdit')).toBeInTheDocument();
    });
});

describe('Functionality tests', () => {
    it('calls setCubeQuery with the correct updated structure when a name is edited', () => {
        const setCubeQuery = jest.fn();
        renderEditor(mockDimension, mockDimEdits, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('bar'), { target: { value: 'editedValue' } });
        expect(resolveCubeQueryUpdate(setCubeQuery, mockDimEdits)).toEqual({
            chartHeaderEdit: {},
            variableQueries: {
                foo: {
                    valueEdits: {
                        bar: {
                            nameEdit: { fi: 'editedValue', sv: 'bar', en: 'bar' }
                        }
                    }
                }
            }
        });
    });

    it('creates the correct edit structure when there are no pre-existing dimension edits', () => {
        const setCubeQuery = jest.fn();
        const emptyQuery: ICubeQuery = { chartHeaderEdit: {}, variableQueries: {} };
        renderEditor(mockDimension, emptyQuery, setCubeQuery);
        fireEvent.change(screen.getByDisplayValue('fgfgfg'), { target: { value: 'newName' } });
        expect(resolveCubeQueryUpdate(setCubeQuery, emptyQuery)).toEqual({
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