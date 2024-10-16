import React from 'react';
import { render } from '@testing-library/react';
import { IValueSelectProps, ValueSelect } from './ValueSelect';
import '@testing-library/jest-dom';
import { EDimensionType } from 'types/cubeMeta';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { IVariable } from '../../types/visualizationResponse';

const mockFunction = jest.fn((value: string) => {
    return;
});

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const mockVariable: IVariable = 
    {
        code: "foobar1",
        name: { fi: "foo1", sv: "bar1", en: "foobar1" },
        note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
        type: EDimensionType.Other,
        values: [
            {
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false
            },
            {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false
            }
        ]
    };

const mockData: IValueSelectProps = {
    variable: mockVariable,
    multiselect: true,
    activeSelections: ['asd1', 'asd2'],
    onValueChanged: mockFunction,
}

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <ValueSelect variable={mockData.variable} activeSelections={mockData.activeSelections} onValueChanged={mockData.onValueChanged} multiselect={mockData.multiselect} />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});