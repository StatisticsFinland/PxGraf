import React from 'react';
import { render } from "@testing-library/react";
import AllVariableSelection from "./AllVariableSelection";

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
        const { asFragment } = render(<AllVariableSelection
        ></AllVariableSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});