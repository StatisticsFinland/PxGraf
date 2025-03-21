import React from 'react';
import { render } from "@testing-library/react";
import { IDimensionValue } from "../../../types/cubeMeta";
import StartingFromDimensionSelection from "./StartingFromDimensionSelection";

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
        const { asFragment } = render(<StartingFromDimensionSelection
            options={mockDimensionValues}
            startingCode={"2019"}
            onQueryChanged={(newCode) => null}
        ></StartingFromDimensionSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});