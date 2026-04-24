import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { LoadingDialogContent } from './LoadingDialogContent';

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<LoadingDialogContent />);
        expect(asFragment()).toMatchSnapshot();
    });
});