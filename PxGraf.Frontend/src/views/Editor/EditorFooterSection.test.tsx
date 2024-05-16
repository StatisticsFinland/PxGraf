import { render } from '@testing-library/react';
import React from 'react';
import EditorFooterSection from './EditorFooterSection';

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => {}),
            },
        };
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<EditorFooterSection />);
        expect(asFragment()).toMatchSnapshot();
    });
});
