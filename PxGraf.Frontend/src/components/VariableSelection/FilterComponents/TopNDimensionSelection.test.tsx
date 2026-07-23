import React from 'react';
import { fireEvent, render, screen } from "@testing-library/react";
import '@testing-library/jest-dom';

import TopNDimensionSelection from "./TopNDimensionSelection";

const mockChangeFunction = jest.fn();

beforeEach(() => {
    mockChangeFunction.mockClear();
});

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={() => { }}
        ></TopNDimensionSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('updates the displayed value when the prop changes', () => {
        const { rerender } = render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        />);

        rerender(<TopNDimensionSelection
            numberOfItems={5}
            onNumberChanged={mockChangeFunction}
        />);

        expect(screen.getByLabelText('variableSelect.latestValuesCountLabel')).toHaveValue('5');
    });

    it('should call onchange with the correct value when value is convertable to number', () => {
        render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNDimensionSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.latestValuesCountLabel'), { target: { value: '5' } });
        expect(mockChangeFunction).toHaveBeenCalledWith(5);
    });

    it('shows an associated error and does not update the query for invalid input', () => {
        render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNDimensionSelection>);
        const input = screen.getByLabelText('variableSelect.latestValuesCountLabel');
        fireEvent.change(input, { target: { value: 'eivoikaantaa' } });

        const error = screen.getByText('variableSelect.latestValuesCountError');
        expect(input).toHaveValue('eivoikaantaa');
        expect(input).toHaveAttribute('aria-invalid', 'true');
        expect(input.getAttribute('aria-describedby')).toContain(error.id);
        expect(mockChangeFunction).not.toHaveBeenCalled();
    });
});