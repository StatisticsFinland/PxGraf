import { fireEvent, render, screen } from '@testing-library/react';
import React from 'react';
import SuccessDialogContent from './SuccessDialogContent';

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

Object.defineProperty(navigator, 'clipboard', {
    value: {
        writeText: jest.fn(),
    },
});

describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const {asFragment} = render(<SuccessDialogContent queryId={'asd123'} />);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(<SuccessDialogContent />);
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('Should call clipboard writeText with query id', () => {
        const queryIdMock = 'asd123';
        render(<SuccessDialogContent queryId={queryIdMock} />);
        fireEvent.click(screen.getByRole('button'));
        expect(navigator.clipboard.writeText).toHaveBeenCalledWith(queryIdMock);
    });
});