import React from 'react';
import { fireEvent, render, screen } from "@testing-library/react";

import TopNDimensionSelection from "./TopNDimensionSelection";

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

const mockChangeFunction = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={(_newValue: number) => { }}
        ></TopNDimensionSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('should call onchange with the correct value when value is convertable to number', () => {
        render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNDimensionSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.topFilter'), { target: { value: '5' } });
        expect(mockChangeFunction).toHaveBeenCalledWith(5);
    });

    it('should call onchange with 0 when value is not convertable to number', () => {
        render(<TopNDimensionSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNDimensionSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.topFilter'), { target: { value: 'eivoikaantaa' } });
        expect(mockChangeFunction).toHaveBeenCalledWith(0);
    });
});