import React from 'react';
import { render } from "@testing-library/react";

import SelectabilitySwitch from "./SelectabilitySwitch";

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
        const { asFragment } = render(<SelectabilitySwitch
            selected={true}
            onChange={jest.fn()}
        ></SelectabilitySwitch>);
        expect(asFragment()).toMatchSnapshot();
    });
});