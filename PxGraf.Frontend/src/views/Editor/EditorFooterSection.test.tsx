import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import EditorFooterSection from './EditorFooterSection';

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<EditorFooterSection />);
        expect(asFragment()).toMatchSnapshot();
    });
});
