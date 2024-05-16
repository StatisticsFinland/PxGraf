import { render } from '@testing-library/react';
import React from 'react';
import { ErrorDialogContent } from './ErrorDialogContent';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => { }),
            },
        };
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<ErrorDialogContent />);
        expect(asFragment()).toMatchSnapshot();
    });
});