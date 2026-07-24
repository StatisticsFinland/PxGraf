import React from 'react';
import { render, screen } from "@testing-library/react";
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';

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

describe('Functionality test', () => {
    it('calls onChange with true when toggled on', async () => {
        const user = userEvent.setup();
        const onChange = jest.fn();

        render(<SelectabilitySwitch selected={false} onChange={onChange} />);

        await user.click(screen.getByRole('switch', { name: 'variableSelect.selectable' }));

        expect(onChange).toHaveBeenCalledWith(true);
    });

    it('calls onChange with false when toggled off', async () => {
        const user = userEvent.setup();
        const onChange = jest.fn();

        render(<SelectabilitySwitch selected={true} onChange={onChange} />);

        await user.click(screen.getByRole('switch', { name: 'variableSelect.selectable' }));

        expect(onChange).toHaveBeenCalledWith(false);
    });
});