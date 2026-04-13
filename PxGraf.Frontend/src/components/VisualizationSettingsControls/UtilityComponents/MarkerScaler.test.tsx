import React from 'react';
import '@testing-library/jest-dom';
import { render, screen, fireEvent } from '@testing-library/react';
import MarkerScaler from "./MarkerScaler";
import { IVisualizationSettings } from '../../../types/visualizationSettings';
import { VisualizationContext } from '../../../contexts/visualizationContext';
import { VisualizationType } from '../../../types/visualizationType';
import { IVisualizationOptions } from '../../../types/editorContentsResponse';

const mockVisualizationRules: IVisualizationOptions = {
    allowManualPivot: null,
    sortingOptions: null,
    allowMultiselect: null
} as unknown as IVisualizationOptions;

const mockVisualizationSettings: IVisualizationSettings = {
    defaultSelectableVariableCodes: null,
    pivotRequested: null,
    cutYAxis: false,
    multiselectableVariableCode: null,
    rowVariableCodes: null,
    columnVariableCodes: null,
    sorting: null,
    matchXLabelsToEnd: null,
    markerSize: 100,
}

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(
            <MarkerScaler
                visualizationOptions={mockVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
            />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Marker size value should be 123 when the corresponding parameter value is 123', () => {
        const toggledMockSettings = { ...mockVisualizationSettings, markerSize: 123 }
        render(
            <MarkerScaler
                visualizationOptions={mockVisualizationRules}
                visualizationSettings={toggledMockSettings}
            />);
        expect(screen.getByDisplayValue('123')).toHaveValue('123');
    });

    it('MarkerScaler onChange and onChangeCommitted should work properly', () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <VisualizationContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: mockSettingsChangedHandler,
            }}>
                <MarkerScaler
                    visualizationOptions={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                />
            </VisualizationContext.Provider>
        );

        const slider = screen.getByRole('slider');
        fireEvent.change(slider, { target: { value: 50 } });
        expect(slider).toHaveValue('50');

        fireEvent.mouseUp(slider);
        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({ ...mockVisualizationSettings, markerSize: 50 });
    });
});