import React from 'react';
import { fireEvent, render } from "@testing-library/react";
import VisualizationSettingsSwitch from "./VisualizationSettingsSwitch";
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
            <VisualizationSettingsSwitch
                selected={false}
                visualizationSettings={mockVisualizationSettings}
                label={"label"}
                changeProperty={"showDataPoints"}
                />
            </EditorContext.Provider>);
        const switchElement = getByRole('checkbox');
        fireEvent.click(switchElement);

        expect(mockSettingsChangedHandler).toHaveBeenCalledWith({ ...mockVisualizationSettings, showDataPoints: true });
    });
});