import React from 'react';
import { VisualizationContext, VisualizationProvider } from 'contexts/visualizationContext';
import { act, render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { VisualizationType } from 'types/visualizationType';

const TestComponent = () => {
    const {
        selectedVisualizationUserInput, setSelectedVisualizationUserInput,
        visualizationSettingsUserInput, setVisualizationSettingsUserInput,
        defaultSelectables, setDefaultSelectables,
    } = React.useContext(VisualizationContext);

    return (
        <>
            <div data-testid="visualization">{selectedVisualizationUserInput ?? 'null'}</div>
            <div data-testid="settings">{visualizationSettingsUserInput ? JSON.stringify(visualizationSettingsUserInput) : 'null'}</div>
            <div data-testid="selectables">{defaultSelectables ? JSON.stringify(defaultSelectables) : 'null'}</div>
            <button
                data-testid="setVisualization"
                onClick={() => setSelectedVisualizationUserInput(VisualizationType.LineChart)}
            />
            <button
                data-testid="setSettings"
                onClick={() => setVisualizationSettingsUserInput({ cutYAxis: true, showDataPoints: false })}
            />
            <button
                data-testid="setSelectables"
                onClick={() => setDefaultSelectables({ dim1: ['val1', 'val2'] })}
            />
        </>
    );
};

describe('VisualizationContext', () => {
    it('should have null values initially', () => {
        const { getByTestId } = render(
            <VisualizationProvider><TestComponent /></VisualizationProvider>
        );
        expect(getByTestId('visualization')).toHaveTextContent('null');
        expect(getByTestId('settings')).toHaveTextContent('null');
        expect(getByTestId('selectables')).toHaveTextContent('null');
    });

    it('should update selectedVisualizationUserInput', () => {
        const { getByTestId } = render(
            <VisualizationProvider><TestComponent /></VisualizationProvider>
        );

        act(() => {
            getByTestId('setVisualization').click();
        });

        expect(getByTestId('visualization')).toHaveTextContent('LineChart');
    });

    it('should update visualizationSettingsUserInput', () => {
        const { getByTestId } = render(
            <VisualizationProvider><TestComponent /></VisualizationProvider>
        );

        act(() => {
            getByTestId('setSettings').click();
        });

        expect(getByTestId('settings')).toHaveTextContent('"cutYAxis":true');
        expect(getByTestId('settings')).toHaveTextContent('"showDataPoints":false');
    });

    it('should update defaultSelectables', () => {
        const { getByTestId } = render(
            <VisualizationProvider><TestComponent /></VisualizationProvider>
        );

        act(() => {
            getByTestId('setSelectables').click();
        });

        expect(getByTestId('selectables')).toHaveTextContent('"dim1"');
        expect(getByTestId('selectables')).toHaveTextContent('"val1"');
    });

    it('should handle multiple state updates independently', () => {
        const { getByTestId } = render(
            <VisualizationProvider><TestComponent /></VisualizationProvider>
        );

        act(() => {
            getByTestId('setVisualization').click();
        });

        // Only visualization changed, others remain null
        expect(getByTestId('visualization')).toHaveTextContent('LineChart');
        expect(getByTestId('settings')).toHaveTextContent('null');
        expect(getByTestId('selectables')).toHaveTextContent('null');

        act(() => {
            getByTestId('setSettings').click();
        });

        // Visualization still set, settings updated, selectables still null
        expect(getByTestId('visualization')).toHaveTextContent('LineChart');
        expect(getByTestId('settings')).toHaveTextContent('"cutYAxis":true');
        expect(getByTestId('selectables')).toHaveTextContent('null');
    });
});
