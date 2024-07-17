import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { HashRouter } from 'react-router-dom';
import { SavedQueryFinder } from './SavedQueryFinder';

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

jest.mock('envVars', () => {
    return {
        PxGrafUrl: 'test-url.fi/',
    };
})

describe('Rendering test', () => {
    it('renders correctly with closed dialog', () => {
        const {baseElement} = render(<HashRouter><SavedQueryFinder /></HashRouter>);
        expect(baseElement).toMatchSnapshot();
    });

    it('renders correctly with open dialog', () => {
        const {baseElement} = render(<HashRouter><SavedQueryFinder /></HashRouter>);
        fireEvent.click(screen.getByText('savedQuery.dialogButtonTxt'));
        expect(baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('Renders dialog if opened, eliminates dialog if closed', () => {
        render(<HashRouter><SavedQueryFinder /></HashRouter>);
        fireEvent.click(screen.getByText('savedQuery.dialogButtonTxt'));
        expect(screen.getByText('savedQuery.dialogTitleTxt')).toBeInTheDocument();
        fireEvent.click(screen.getByText('savedQuery.closeButton'));
        setTimeout(() => {
            expect(screen.queryByText('savedQuery.dialogTitleTxt')).toBeFalsy();
        }, 1000);
    });

    it('Renders initial input in the input field', () => {
        render(<HashRouter><SavedQueryFinder oldQueryId='foobar' /></HashRouter>);
        fireEvent.click(screen.getByText('savedQuery.dialogButtonTxt'));
        expect(screen.getByDisplayValue('foobar')).toBeInTheDocument();
    });
});