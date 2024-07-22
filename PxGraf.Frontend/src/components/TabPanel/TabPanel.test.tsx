import React from 'react';
import { render } from '@testing-library/react';
import { TabPanel } from './TabPanel';
import '@testing-library/jest-dom';

const mockChildren = ['foo', 'bar', 'baz'];

describe('Rendering test', () => {
    it('renders correctly with selected value', () => {
        // mock router for testing purposes
        const { asFragment } = render(<TabPanel value={'1'} selectedValue={'1'} children={mockChildren} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly with not selected value', () => {
        // mock router for testing purposes
        const { asFragment } = render(<TabPanel value={'1'} selectedValue={'2'} children={mockChildren} />);
        expect(asFragment()).toMatchSnapshot();
    });
});