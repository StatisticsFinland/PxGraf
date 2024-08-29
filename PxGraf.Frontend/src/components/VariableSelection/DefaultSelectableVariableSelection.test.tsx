import React from 'react';
import { render } from "@testing-library/react";
import { IDimensionValue } from "../../types/cubeMeta";
import DefaultSelectableVariableSelection from "./DefaultSelectableVariableSelection";

const mockVariableValues: IDimensionValue[] = [
    {
        Code: "2018",
        Name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        Virtual: false,
    },
    {
        Code: "2019",
        Name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        Virtual: false,
    },
    {
        Code: "2020",
        Name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        Virtual: false,
    },
    {
        Code: "2021",
        Name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        Virtual: false,
    }
];

const mockResolvedCodes: string[] = ['2018', '2019'];

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
        const { asFragment } = render(<DefaultSelectableVariableSelection
            options={mockVariableValues}
            resolvedVariableValueCodes={mockResolvedCodes}
            variableCode={'foo'}
        ></DefaultSelectableVariableSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});