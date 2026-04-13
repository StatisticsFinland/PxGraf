import React from 'react';
import { render, screen } from '@testing-library/react';
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

describe('Assertion tests', () => {
    it('renders a tabpanel role element', () => {
        render(<TabPanel value={'1'} selectedValue={'1'} children={mockChildren} />);
        expect(screen.getByRole('tabpanel')).toBeInTheDocument();
    });

    it('displays children when value matches selectedValue', () => {
        render(<TabPanel value={'1'} selectedValue={'1'} children={<span>test content</span>} />);
        expect(screen.getByText('test content')).toBeInTheDocument();
    });

    it('hides content when value does not match selectedValue', () => {
        render(<TabPanel value={'1'} selectedValue={'2'} children={<span>test content</span>} />);
        expect(screen.queryByText('test content')).not.toBeInTheDocument();
    });

    it('sets the hidden attribute when not selected', () => {
        const { container } = render(<TabPanel value={'1'} selectedValue={'2'} children={mockChildren} />);
        const panel = container.querySelector('[role="tabpanel"]');
        expect(panel).toHaveAttribute('hidden');
    });

    it('does not set hidden attribute when selected', () => {
        const { container } = render(<TabPanel value={'1'} selectedValue={'1'} children={mockChildren} />);
        const panel = container.querySelector('[role="tabpanel"]');
        expect(panel).not.toHaveAttribute('hidden');
    });

    it('sets correct id and aria-labelledby attributes', () => {
        const { container } = render(<TabPanel value={'test'} selectedValue={'test'} children={mockChildren} />);
        const panel = container.querySelector('[role="tabpanel"]');
        expect(panel).toHaveAttribute('id', 'simple-tabpanel-test');
        expect(panel).toHaveAttribute('aria-labelledby', 'simple-tab-test');
    });
});