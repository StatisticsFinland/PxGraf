import React from 'react';
import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import { ErrorDialogContent } from './ErrorDialogContent';

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<ErrorDialogContent />);
        expect(asFragment()).toMatchSnapshot();
    });
});