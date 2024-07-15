import { render } from "@testing-library/react";

import StartingFromVariableSelection from "./StartingFromVariableSelection";

const mockVariableValues = [
    {
        code: "2018",
        name: {
            fi: "2018",
            sv: "2018",
            en: "2018"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2019",
        name: {
            fi: "2019",
            sv: "2019",
            en: "2019"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2020",
        name: {
            fi: "2020",
            sv: "2020",
            en: "2020"
        },
        note: null,
        isSum: false,
        contentComponent: null
    },
    {
        code: "2021",
        name: {
            fi: "2021*",
            sv: "2021*",
            en: "2021*"
        },
        note: null,
        isSum: false,
        contentComponent: null
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
        const { asFragment } = render(<StartingFromVariableSelection
            options={mockVariableValues}
            startingCode={"2019"}
            onQueryChanged={(newCode) => null}
        ></StartingFromVariableSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});