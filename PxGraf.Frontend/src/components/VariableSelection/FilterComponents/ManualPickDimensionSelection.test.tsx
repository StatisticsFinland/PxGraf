import React from 'react';
import { render } from "@testing-library/react";
import ManualPickDimensionSelection from "./ManualPickDimensionSelection";
import UiLanguageContext from "contexts/uiLanguageContext";
import { IDimensionValue } from "../../../types/cubeMeta";
import userEvent from '@testing-library/user-event';

const mockDimensionValues: IDimensionValue[] = [
    {
        code: "2018",
        name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        isVirtual: false,
    },
    {
        code: "2019",
        name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        isVirtual: false,
    },
    {
        code: "2020",
        name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        isVirtual: false,
    },
    {
        code: "2021",
        name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        isVirtual: false,
    }
];

const setLanguage = jest.fn();
const language = 'fi';

const setLanguageTab = jest.fn();
const languageTab = 'fi';

const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = "fi";
const setUiContentLanguage = jest.fn();

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
        <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
            <ManualPickDimensionSelection
                options={mockDimensionValues}
                selectedValues={[mockDimensionValues[0], mockDimensionValues[1]]}
                onQueryChanged={() => {}}/>
        </UiLanguageContext.Provider>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Functionality test', () => {
    it('calls the onQueryChanged function when a value is selected', async () => {
        const user = userEvent.setup();
        const mockOnQueryChanged = jest.fn();

        const { getByLabelText, findByRole } = render(
            <UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}>
                <ManualPickDimensionSelection
                    options={mockDimensionValues}
                    selectedValues={[mockDimensionValues[0]]}
                    onQueryChanged={mockOnQueryChanged} />
            </UiLanguageContext.Provider>
        );
        const selectableMenu = getByLabelText('variableSelect.itemFilter');
        await user.click(selectableMenu);
        const valueSelect = await findByRole('option', { name: '2020' });
        await user.click(valueSelect);

        expect(mockOnQueryChanged).toHaveBeenCalledWith(["2018", "2020"]);
    })
});