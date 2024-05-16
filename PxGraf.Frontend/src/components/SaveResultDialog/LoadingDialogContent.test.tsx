import { render } from '@testing-library/react';
import React from 'react';
import { LoadingDialogContent } from './LoadingDialogContent';

describe('Rendering test', () => {
    it('renders correctly', () => {
        // for some
        const { asFragment } = render(<LoadingDialogContent />);
        expect(asFragment()).toMatchSnapshot();
    });
});