import React from 'react';
import { act, fireEvent, render, screen } from "@testing-library/react";
import '@testing-library/jest-dom';

import RegexDimensionSelection from "./RegexDimensionSelection";

const mockChangeFunction = jest.fn();

beforeEach(() => {
    mockChangeFunction.mockClear();
    jest.useFakeTimers();
});

afterEach(() => {
    jest.useRealTimers();
});

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<RegexDimensionSelection
            pattern="^val"
            onQueryChanged={() => { }}
        ></RegexDimensionSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('updates the displayed value when the prop changes', () => {
        const { rerender } = render(<RegexDimensionSelection
            pattern="^val"
            onQueryChanged={mockChangeFunction}
        />);

        rerender(<RegexDimensionSelection
            pattern="^new"
            onQueryChanged={mockChangeFunction}
        />);

        expect(screen.getByLabelText('variableSelect.regexPatternLabel')).toHaveValue('^new');
    });

    it('calls onQueryChanged with the new pattern when a valid pattern is entered', () => {
        render(<RegexDimensionSelection
            pattern="^val"
            onQueryChanged={mockChangeFunction}
        ></RegexDimensionSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.regexPatternLabel'), { target: { value: 'val[0-9]' } });

        expect(mockChangeFunction).not.toHaveBeenCalled();

        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(mockChangeFunction).toHaveBeenCalledWith('val[0-9]');
    });

    it('debounces rapid changes so only the last pattern is applied', () => {
        render(<RegexDimensionSelection
            pattern="^val"
            onQueryChanged={mockChangeFunction}
        ></RegexDimensionSelection>);
        const input = screen.getByLabelText('variableSelect.regexPatternLabel');

        fireEvent.change(input, { target: { value: 'val[0-9]' } });
        fireEvent.change(input, { target: { value: 'val[0-9]+' } });

        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(mockChangeFunction).toHaveBeenCalledTimes(1);
        expect(mockChangeFunction).toHaveBeenCalledWith('val[0-9]+');
    });

    it('shows an associated error but still calls onQueryChanged for invalid input', () => {
        render(<RegexDimensionSelection
            pattern="^val"
            onQueryChanged={mockChangeFunction}
        ></RegexDimensionSelection>);
        const input = screen.getByLabelText('variableSelect.regexPatternLabel');
        fireEvent.change(input, { target: { value: '[invalid' } });

        const error = screen.getByText('variableSelect.regexPatternError');
        expect(input).toHaveValue('[invalid');
        expect(input).toHaveAttribute('aria-invalid', 'true');
        expect(input.getAttribute('aria-describedby')).toContain(error.id);

        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(mockChangeFunction).toHaveBeenCalledWith('[invalid');
    });
});
