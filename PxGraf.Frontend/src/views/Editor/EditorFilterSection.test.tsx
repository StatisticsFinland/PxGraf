import React from 'react';
import { fireEvent, render, screen } from "@testing-library/react";
import '@testing-library/jest-dom';
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

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const onEditMetadata = jest.fn();
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorFilterSection
                    queries={mockQuery}
                    resolvedDimensionCodes={{ 'foo': ['foo', 'bar', 'baz'] }}
                    dimensions={mockDimensions}
                    width={320}
                    maxWidthPercentage={33}
                    onEditMetadata={onEditMetadata}
                    />
            </UiLanguageContext.Provider>
        );
        fireEvent.click(screen.getByRole('button', { name: 'editMetadata.dialogTitle' }));
        expect(onEditMetadata).toHaveBeenCalledTimes(1);
        expect(asFragment()).toMatchSnapshot();
    });

    it('disables metadata editing when no values are selected', () => {
        render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <EditorFilterSection
                    queries={mockQuery}
                    resolvedDimensionCodes={{ 'foo': [] }}
                    dimensions={mockDimensions}
                    width={320}
                    maxWidthPercentage={33}
                    onEditMetadata={jest.fn()}
                />
            </UiLanguageContext.Provider>
        );

        expect(screen.getByRole('button', { name: 'editMetadata.dialogTitle' })).toBeDisabled();
    });
});