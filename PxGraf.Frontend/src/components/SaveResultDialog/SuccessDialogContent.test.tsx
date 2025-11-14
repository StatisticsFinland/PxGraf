import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import SuccessDialogContent from './SuccessDialogContent';
import { EQueryPublicationStatus } from "types/saveQuery";

const mockT = jest.fn((key: string) => {
    const translations: { [key: string]: string } = {
        'saveResultDialog.id': 'Selection identifier',
        'saveResultDialog.copyToClipBoard': 'Copy to clipboard',
        'error.webhookResponseError': 'Webhook response error'
    };
    return translations[key] || key;
});

const mockI18n = {
    language: 'en',
    changeLanguage: () => new Promise(() => { })
};

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => ({
        t: mockT,
        i18n: mockI18n,
    }),
}));

Object.defineProperty(navigator, 'clipboard', {
    value: {
        writeText: jest.fn(),
    },
});

describe('Rendering test', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        mockI18n.language = 'en'; // Reset language before each test
    });

    it('renders correctly when closed', () => {
        const { asFragment } = render(<SuccessDialogContent queryId={'asd123'} />);
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders correctly when open (draft)', () => {
        const dom = render(<SuccessDialogContent isDraft={true} publicationStatus={EQueryPublicationStatus.Unpublished} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly with webhook success message', () => {
        const publicationMessage = {
            'en': 'Publication was successful',
            'fi': 'Julkaisu onnistui',
            'sv': 'Publicering lyckades'
        };
        const dom = render(
            <SuccessDialogContent
                isDraft={false}
                publicationStatus={EQueryPublicationStatus.Success}
                publicationMessage={publicationMessage}
            />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly with webhook error message', () => {
        const publicationMessage = {
            'error': 'Invalid response format: JSON parsing failed'
        };
        const dom = render(
            <SuccessDialogContent
                isDraft={false}
                publicationStatus={EQueryPublicationStatus.Failed}
                publicationMessage={publicationMessage}
            />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Publication message functionality tests', () => {
    beforeEach(() => {
        jest.clearAllMocks();
        mockI18n.language = 'en';
    });

    it('displays webhook message for current language when available', () => {
        const publicationMessage = {
            'en': 'Publication was successful',
            'fi': 'Julkaisu onnistui',
            'sv': 'Publicering lyckades'
        };

        render(
            <SuccessDialogContent
                isDraft={false}
                publicationStatus={EQueryPublicationStatus.Success}
                publicationMessage={publicationMessage}
            />
        );

        expect(screen.getByText('Publication was successful')).toBeInTheDocument();
    });

    it('displays webhook message in fallback language when current language not available', () => {
        mockI18n.language = 'de'; // Language not in the message

        const publicationMessage = {
            'en': 'Publication was successful',
            'fi': 'Julkaisu onnistui'
        };

        render(
            <SuccessDialogContent
                isDraft={false}
                publicationStatus={EQueryPublicationStatus.Success}
                publicationMessage={publicationMessage}
            />
        );

        // Should fall back to the first available language (en)
        expect(screen.getByText('Publication was successful')).toBeInTheDocument();
    });

    it('displays error message directly when error key is present', () => {
        const publicationMessage = {
            'error': 'Generic error message'
        };

        render(
            <SuccessDialogContent
                isDraft={false}
                publicationStatus={EQueryPublicationStatus.Failed}
                publicationMessage={publicationMessage}
            />
        );

        // Should display the error message directly since it's not one of the mapped errors
        expect(screen.getByText('Generic error message')).toBeInTheDocument();
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