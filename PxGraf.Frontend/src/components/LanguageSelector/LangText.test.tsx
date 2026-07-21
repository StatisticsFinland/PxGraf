import React from 'react';
import { render, screen } from '@testing-library/react';
import LangText from './LangText';
import '@testing-library/jest-dom';

describe('LangText', () => {
    it('renders text', () => {
        render(<LangText text="English" />);
        const element = screen.getByText('English');
        expect(element).toBeInTheDocument();
        expect(element.tagName).toBe('P');
    });

    it('renders empty string text', () => {
        const { container } = render(<LangText text="" />);
        expect(container.querySelector('p')).toBeInTheDocument();
        expect(container.querySelector('p').textContent).toBe('');
    });
});
