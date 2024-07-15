import '@testing-library/jest-dom';
import { render, screen, fireEvent } from '@testing-library/react';
import MarkerScaler from "./MarkerScaler";

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

const mockVisualizationRules = {
    allowManualPivot: null,
    sortingOptions: null,
    multiselectVariableAllowed: null
};

const mockVisualizationSettings = {
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
        const mockSettingsChangedHandler = jest.fn();
        const { asFragment } = render(
            <MarkerScaler
                visualizationRules={mockVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
            />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Marker size value should be 123 when the corresponding parameter value is 123', () => {
        const mockSettingsChangedHandler = jest.fn();
        const toggledMockSettings = { ...mockVisualizationSettings, markerSize: 123 }
        render(
            <MarkerScaler
                visualizationRules={mockVisualizationRules}
                visualizationSettings={toggledMockSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
            />);
        expect(screen.getByDisplayValue('123')).toHaveValue('123');
    });

    it('MarkerScaler onChange and onChangeCommitted should work properly', () => {
        const mockSettingsChangedHandler = jest.fn();
        render(
            <MarkerScaler
                visualizationRules={mockVisualizationRules}
                visualizationSettings={mockVisualizationSettings}
                settingsChangedHandler={mockSettingsChangedHandler}
            />
        );

        const slider = screen.getByRole('slider');
        fireEvent.change(slider, { target: { value: 50 } });
        expect(slider).toHaveValue('50');

        fireEvent.mouseUp(slider);
        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({ ...mockVisualizationSettings, markerSize: 50 });
    });
});