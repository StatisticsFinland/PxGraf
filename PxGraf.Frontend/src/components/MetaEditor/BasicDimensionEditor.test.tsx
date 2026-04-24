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