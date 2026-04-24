import React from 'react';
import { render } from "@testing-library/react";

import SelectabilitySwitch from "./SelectabilitySwitch";

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<SelectabilitySwitch
            selected={true}
            onChange={jest.fn()}
        ></SelectabilitySwitch>);
        expect(asFragment()).toMatchSnapshot();
    });
});