import React from 'react';
import { render, screen } from '@testing-library/react';
import LangText from './LangText';
import '@testing-library/jest-dom';

describe('LangText', () => {
    it('renders text with underline when underline is true', () => {
        render(<LangText text="English" underline={true} />);
        const element = screen.getByText('English');
        expect(element).toBeInTheDocument();
        expect(element.tagName).toBe('P');
        expect(element).toHaveStyle('text-decoration: underline');
    });

    it('renders text without underline when underline is false', () => {
        render(<LangText text="Suomi" underline={false} />);
        const element = screen.getByText('Suomi');
        expect(element).toBeInTheDocument();
        expect(element.tagName).toBe('P');
        expect(element).not.toHaveStyle('text-decoration: underline');
    });

    it('renders empty string text', () => {
        const { container } = render(<LangText text="" underline={false} />);
        expect(container.querySelector('p')).toBeInTheDocument();
        expect(container.querySelector('p').textContent).toBe('');
    });
});
