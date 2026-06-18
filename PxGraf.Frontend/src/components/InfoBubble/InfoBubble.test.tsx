import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import InfoBubble from './InfoBubble';

describe('Rendering test', () => {
    it('renders correctly with popper hidden', () => {
        const { asFragment } = render(<InfoBubble info='foobar' ariaLabel='foo' />);
        expect(asFragment()).toMatchSnapshot();
    });

    it('applies the id prop to the button', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' id='my-info' />);
        expect(screen.getByRole('button')).toHaveAttribute('id', 'my-info');
    });

    it('renders a React node as info content', () => {
        render(<InfoBubble info={<span data-testid='node-info'>rich content</span>} ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        expect(screen.getByTestId('node-info')).toBeInTheDocument();
    });

    it('popper has role alert', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        expect(screen.getByRole('alert', { hidden: true })).toBeInTheDocument();
    });
});

describe('Assertion test', () => {
    it('renders popper when clicked', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        expect(screen.getByText('foobar')).toBeInTheDocument();
    });

    it('renders popper when mouse enters', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.mouseEnter(screen.getByRole('button'));
        expect(screen.getByText('foobar')).toBeInTheDocument();
    });

    it('hides popper when mouse leaves the button', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        const button = screen.getByRole('button');
        fireEvent.mouseEnter(button);
        expect(screen.getByText('foobar')).toBeVisible();
        fireEvent.mouseLeave(button);
        expect(screen.getByText('foobar')).not.toBeVisible();
    });

    it('toggles popper closed on second click', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        const button = screen.getByRole('button');
        fireEvent.click(button);
        expect(screen.getByText('foobar')).toBeVisible();
        fireEvent.click(button);
        expect(screen.getByText('foobar')).not.toBeVisible();
    });

    it('closes popper when Escape key is pressed', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        expect(screen.getByText('foobar')).toBeVisible();
        fireEvent.keyDown(window, { key: 'Escape' });
        expect(screen.getByText('foobar')).not.toBeVisible();
    });

    it('does not close popper on non-Escape key press', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        fireEvent.keyDown(window, { key: 'Enter' });
        expect(screen.getByText('foobar')).toBeVisible();
    });

    it('Escape key has no effect when popper is already closed', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        // popper is closed by default; pressing Escape should not throw
        expect(() => fireEvent.keyDown(window, { key: 'Escape' })).not.toThrow();
        expect(screen.getByText('foobar')).not.toBeVisible();
    });

    it('keeps popper open when mouse enters the popper', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        fireEvent.mouseEnter(screen.getByRole('alert'));
        expect(screen.getByText('foobar')).toBeVisible();
    });

    it('hides popper when mouse leaves the popper', () => {
        render(<InfoBubble info='foobar' ariaLabel='foo' />);
        fireEvent.click(screen.getByRole('button'));
        fireEvent.mouseLeave(screen.getByRole('alert'));
        expect(screen.getByText('foobar')).not.toBeVisible();
    });
});

describe('Aria label test', () => {
    it('aria-label indicates closed state initially', () => {
        render(<InfoBubble info='foobar' ariaLabel='my label' />);
        expect(screen.getByRole('button')).toHaveAttribute('aria-label', 'tooltip.open: my label');
    });

    it('aria-label indicates open state after click', () => {
        render(<InfoBubble info='foobar' ariaLabel='my label' />);
        fireEvent.click(screen.getByRole('button'));
        expect(screen.getByRole('button')).toHaveAttribute('aria-label', 'tooltip.close: my label');
    });

    it('aria-label reverts to closed state after second click', () => {
        render(<InfoBubble info='foobar' ariaLabel='my label' />);
        const button = screen.getByRole('button');
        fireEvent.click(button);
        fireEvent.click(button);
        expect(button).toHaveAttribute('aria-label', 'tooltip.open: my label');
    });
});
