import React from 'react';
import { render } from "@testing-library/react";
import AllDimensionSelection from "./AllDimensionSelection";

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<AllDimensionSelection
        ></AllDimensionSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});