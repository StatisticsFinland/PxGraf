import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import SuccessDialogContent from './SuccessDialogContent';
import { EQueryPublicationStatus } from '../../api/services/queries';

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

    it('renders correctly when open (draft)', () => {
        const dom = render(<SuccessDialogContent isDraft={true} publicationStatus={EQueryPublicationStatus.Unpublished} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open (published)', () => {
        const dom = render(<SuccessDialogContent isDraft={false} publicationStatus={EQueryPublicationStatus.Success} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open (publication error)', () => {
        const dom = render(<SuccessDialogContent isDraft={false} publicationStatus={EQueryPublicationStatus.Failed} />);
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