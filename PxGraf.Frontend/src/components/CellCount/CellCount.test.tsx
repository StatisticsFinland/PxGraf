import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import CellCount from './CellCount';

describe('Rendering test', () => {
    it('renders correctly with values within range', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={1} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly with values over warning limit', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={4} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly with values over maximum', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={6} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('shows info severity and correct count when size is within range', () => {
        render(<CellCount maximumSize={5} size={1} warningLimit={3} />);
        const alert = screen.getByRole('alert');
        expect(alert).toHaveClass('MuiAlert-standardInfo');
        expect(alert).toHaveTextContent('cellCount.infoText: 1/5');
    });

    it('shows warning severity when size is at the warning limit', () => {
        render(<CellCount maximumSize={5} size={3} warningLimit={3} />);
        const alert = screen.getByRole('alert');
        expect(alert).toHaveClass('MuiAlert-standardWarning');
        expect(alert).toHaveTextContent('cellCount.warningText: 3/5');
    });

    it('shows warning severity when size is above warning but below max', () => {
        render(<CellCount maximumSize={5} size={4} warningLimit={3} />);
        const alert = screen.getByRole('alert');
        expect(alert).toHaveClass('MuiAlert-standardWarning');
        expect(alert).toHaveTextContent('cellCount.warningText: 4/5');
    });

    it('shows error severity when size exceeds maximum', () => {
        render(<CellCount maximumSize={5} size={6} warningLimit={3} />);
        const alert = screen.getByRole('alert');
        expect(alert).toHaveClass('MuiAlert-standardError');
        expect(alert).toHaveTextContent('cellCount.dangerText: 6/5');
    });

    it('wraps alert in an aria-live polite region', () => {
        const { container } = render(<CellCount maximumSize={5} size={1} warningLimit={3} />);
        const liveRegion = container.querySelector('[aria-live="polite"]');
        expect(liveRegion).toBeInTheDocument();
    });
});