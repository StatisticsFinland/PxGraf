import React from 'react';
import { render } from "@testing-library/react";
import { IDimensionValue } from "../../types/cubeMeta";
import ResultList from "./ResultList";

const mockVariableValues: IDimensionValue[] = [
    {
        Code: "2018",
        Name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        IsVirtual: false,
    },
    {
        Code: "2019",
        Name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        IsVirtual: false,
    },
    {
        Code: "2020",
        Name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        IsVirtual: false,
    },
    {
        Code: "2021",
        Name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        IsVirtual: false,
    }
];

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
        const { asFragment } = render(<ResultList
            variableValues={mockVariableValues}
            resolvedVariableValueCodes={["2019", "2020"]}
        ></ResultList>);
        expect(asFragment()).toMatchSnapshot();
    });
});