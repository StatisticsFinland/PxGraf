import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import SaveResultDialog from './SaveResultDialog';
import { ISaveQueryResult } from 'api/services/queries';

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

const mockSuccessMutation: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: 0 },
    mutate: function (_property: any): void {
        throw new Error('Function not implemented.');
    }
};

const mockErrorMutation: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: 2 },
    mutate: function (_property: any): void {
        throw new Error('Function not implemented.');
    }
};

const onCloseMock = jest.fn();

describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const dom = render(<SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={false} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(<SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />);
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Error rendering test', () => {
    it('renders error correctly when open', () => {
        const dom = render(<SaveResultDialog mutation={mockErrorMutation} onClose={onCloseMock} open={true} />);
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('invokes close function when cancel button is clicked', () => {
        render(<SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />);
        fireEvent.click(screen.getByText('saveResultDialog.ok'));
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });
});