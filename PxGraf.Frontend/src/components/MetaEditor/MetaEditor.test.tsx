import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { ICubeQuery } from 'types/query';
import MetaEditor from './MetaEditor';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { QueryContext } from 'contexts/queryContext';
import { IEditorContentsResult } from '../../api/services/editor-contents';
import { IEditorContentsResponse } from '../../types/editorContentsResponse';

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const mockLang = 'fi';

const data: IEditorContentsResponse = {
    headerText: {
        'fi': 'asd',
        'en': 'asd',
        'sv': 'asd'
    }
} as unknown as IEditorContentsResponse;

const defaultHeaderResponseMock: IEditorContentsResult = {
    isLoading: false,
    isError: false,
    data: data
}

const mockCubeQuery: ICubeQuery = {
    chartHeaderEdit: {
        'fi': 'foo',
        'sv': 'foo',
        'en': 'foo'
    },
    variableQueries: {
        'asd123query': {
            valueEdits: {
                'code1': {
                    contentComponent: {
                        sourceEdit: {
                            'fi': '123',
                            'sv': '123',
                            'en': '123'
                        },
                        unitEdit: {
                            'fi': '456',
                            'sv': '456',
                            'en': '456'
                        }
                    }
                }
            }
        }
    }
};

const setCubeQuery = jest.fn();
const query = null;
const setQuery = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <QueryContext.Provider value={{ cubeQuery: mockCubeQuery, setCubeQuery, query, setQuery }}>
                    <MetaEditor
                        editorContentsResponse={defaultHeaderResponseMock}
                        language={mockLang}
                    />
                </QueryContext.Provider>
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});