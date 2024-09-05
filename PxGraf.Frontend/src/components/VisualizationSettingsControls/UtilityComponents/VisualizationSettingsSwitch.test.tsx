import React from 'react';
import { fireEvent, render } from "@testing-library/react";

import VisualizationSettingsSwitch from "./VisualizationSettingsSwitch";

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

const mockSettingsChangedHandler = jest.fn();
const mockVisualizationSettings = { showDataPoints: false };

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <VisualizationSettingsSwitch
                selected={true}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
                label={"label"}
                changeProperty={"showDataPoints"}
            />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('calls settingsChangedHandler with correct value when clicked', () => {
        const { getByRole } = render(
            <VisualizationSettingsSwitch
                selected={false}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
                label={"label"}
                changeProperty={"showDataPoints"}
            />);
        const switchElement = getByRole('checkbox');
        fireEvent.click(switchElement);

        expect(mockSettingsChangedHandler).toBeCalledWith({ ...mockVisualizationSettings, showDataPoints: true });
    });
});