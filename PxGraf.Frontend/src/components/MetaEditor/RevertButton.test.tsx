import React from 'react';
import { render, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import RevertButton from "./RevertButton";

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
}));

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<RevertButton onClick={() => undefined} />);
        expect(asFragment()).toMatchSnapshot();
    });

    it('calls onClick handler when clicked', () => {
        const handleClick = jest.fn();
        const { getByRole } = render(<RevertButton onClick={handleClick} />);
        fireEvent.click(getByRole('button'));
        expect(handleClick).toHaveBeenCalledTimes(1);
    });
});