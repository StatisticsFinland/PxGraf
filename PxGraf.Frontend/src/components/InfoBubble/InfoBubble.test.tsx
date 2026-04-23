import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import InfoBubble from './InfoBubble';

describe('Rendering test', () => {
    it('renders correctly with popper hidden', () => {
        const { asFragment } = render(<InfoBubble info='foobar' ariaLabel='foo' />);
        expect(asFragment()).toMatchSnapshot();
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
});
