import React from 'react';
import { fireEvent, render } from "@testing-library/react";
import VisualizationSettingsSwitch from "./VisualizationSettingsSwitch";
import { VisualizationContext } from '../../../contexts/visualizationContext';
import { VisualizationType } from '../../../types/visualizationType';

const mockSettingsChangedHandler = jest.fn();
const mockVisualizationSettings = { showDataPoints: false };

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <VisualizationSettingsSwitch
                selected={true}
                visualizationSettings={mockVisualizationSettings}
                label={"label"}
                changeProperty={"showDataPoints"}
            />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('calls settingsChangedHandler with correct value when clicked', () => {
        const { getByRole } = render(
            <VisualizationContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: mockSettingsChangedHandler,
            }}>
                <VisualizationSettingsSwitch
                    selected={false}
                    visualizationSettings={mockVisualizationSettings}
                    label={"label"}
                    changeProperty={"showDataPoints"}
                />
            </VisualizationContext.Provider>);
        const switchElement = getByRole('switch');
        fireEvent.click(switchElement);

        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({ ...mockVisualizationSettings, showDataPoints: true });
    });
});