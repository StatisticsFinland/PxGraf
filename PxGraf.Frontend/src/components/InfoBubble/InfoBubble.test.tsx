import { fireEvent, render, screen } from '@testing-library/react';
import React from 'react';
import InfoBubble from './InfoBubble';

describe('Rendering test', () => {
    it('renders correctly with popper hidden', () => {
        const { asFragment } = render(<InfoBubble info='foobar' ariaLabel='foo' />);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly with popper visible', () => {
        const { asFragment } = render(<InfoBubble info='foobar' ariaLabel='foo' isOpen={true} />);
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
