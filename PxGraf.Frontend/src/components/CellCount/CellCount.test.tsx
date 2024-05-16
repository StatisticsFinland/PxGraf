import { render } from '@testing-library/react';
import React from 'react';
import CellCount from './CellCount';

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
    it('renders correctly with values within range', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={1} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly with values over warning limit', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={4} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly with values over maximum', () => {
        const { asFragment } = render(<CellCount maximumSize={5} size={6} warningLimit={3} />);
        expect(asFragment()).toMatchSnapshot();
    });
});