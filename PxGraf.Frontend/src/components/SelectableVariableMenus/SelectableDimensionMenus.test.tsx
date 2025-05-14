import React from 'react';
import { render } from '@testing-library/react';
import { ISelectableSelections, SelectableDimensionMenus } from './SelectableDimensionMenus';
import '@testing-library/jest-dom';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { ISelectabilityInfo } from '../Preview/Preview';
import { EDimensionType } from 'types/cubeMeta'; 
import userEvent from '@testing-library/user-event';

const mockSetSelections = jest.fn((selections: ISelectableSelections | ((prevState: ISelectableSelections) => ISelectableSelections)) => {
    return;
});

const mockSelectables: ISelectabilityInfo[] = [
    {
        dimension: {
            code: "foobar1",
            name: { fi: "foo1", sv: "bar1", en: "foobar1" },
            note: { fi: "föö1", sv: "bär1", en: "fööbär1" },
            type: EDimensionType.Other,
            values: [{
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false,
                contentComponent: null,
            }, {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false,
                contentComponent: null,
            }],
        },
        multiselectable: false,
    },
    {
        dimension: {
            code: "foobar2",
            name: { fi: "foo2", sv: "bar2", en: "foobar2" },
            note: { fi: "föö2", sv: "bär2", en: "fööbär2" },
            type: EDimensionType.Other,
            values: [{
                code: "barfoo1",
                name: { fi: "fyy1", sv: "bör1", en: "fyybör1" },
                note: { fi: "fuu1", sv: "baar1", en: "fuubaar1" },
                isSum: false,
                contentComponent: null,
            }, {
                code: "barfoo2",
                name: { fi: "fyy2", sv: "bör2", en: "fyybör2" },
                note: { fi: "fuu2", sv: "baar2", en: "fuubaar2" },
                isSum: false,
                contentComponent: null,
            }],
        },
        multiselectable: false,
    }
]

const mockSelections: ISelectableSelections = {
    foobar1: ['barfoo2'],
    foobar2: ['barfoo1']
}

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <SelectableDimensionMenus
                    setSelections={mockSetSelections}
                    selectables={mockSelectables}
                    selections={mockSelections}
                />
            </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Functionality tests', () => {
    it('calls setSelections when a value is changed', async () => {
        const user = userEvent.setup();
        const expectedSelections: ISelectableSelections = {
            foobar1: ['barfoo2'],
            foobar2: ['barfoo1']
        }
        const { findByRole, getByLabelText } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <SelectableDimensionMenus
                    setSelections={mockSetSelections}
                    selectables={mockSelectables}
                    selections={mockSelections}
                />
            </UiLanguageContext.Provider>
        );
        const selectableMenu = getByLabelText('foo1');
        await user.click(selectableMenu);
        const valueSelect = await findByRole('option', { name: 'fyy2' });
        await user.click(valueSelect);

        expect(mockSetSelections).toHaveBeenCalledWith(expectedSelections);
    });
});