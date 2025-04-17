import React from 'react';
import '@testing-library/jest-dom';
import { render, screen, fireEvent } from '@testing-library/react';
import MarkerScaler from "./MarkerScaler";
import { IVisualizationRules } from '../../../types/visualizationRules';
import { IVisualizationSettings } from '../../../types/visualizationSettings';
import { EditorContext } from '../../../contexts/editorContext';
import { VisualizationType } from '../../../types/visualizationType';

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

const mockVisualizationRules: IVisualizationRules = {
    allowManualPivot: null,
    sortingOptions: null,
    multiselectDimensionAllowed: null
};

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
                visualizationRules={mockVisualizationRules}
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
                visualizationRules={mockVisualizationRules}
                visualizationSettings={toggledMockSettings}
            />);
        expect(screen.getByDisplayValue('123')).toHaveValue('123');
    });

    it('MarkerScaler onChange and onChangeCommitted should work properly', () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <EditorContext.Provider value={{
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: false,
                setSaveDialogOpen: jest.fn(),
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: mockSettingsChangedHandler
            }}>
                <MarkerScaler
                    visualizationRules={mockVisualizationRules}
                    visualizationSettings={mockVisualizationSettings}
                />
            </EditorContext.Provider>
        );

        const slider = screen.getByRole('slider');
        fireEvent.change(slider, { target: { value: 50 } });
        expect(slider).toHaveValue('50');

        fireEvent.mouseUp(slider);
        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({ ...mockVisualizationSettings, markerSize: 50 });
    });
});