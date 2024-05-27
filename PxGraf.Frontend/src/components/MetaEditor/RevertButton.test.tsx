import { render } from '@testing-library/react';
import React from 'react';
import RevertButton from "./RevertButton";

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<RevertButton onClick={() => undefined} />);
        expect(asFragment()).toMatchSnapshot();
    });
});