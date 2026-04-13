import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import SaveResultDialog from './SaveResultDialog';
import { EQueryPublicationStatus } from "types/saveQuery";
import { ISaveQueryMutationParams, ISaveQueryResult } from 'api/services/queries';


const mockSuccessMutation: ISaveQueryResult = {
    isPending: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Success },
    mutate: jest.fn<void, [ISaveQueryMutationParams]>()
};

const mockSuccessDraftMutation: ISaveQueryResult = {
    isPending: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Unpublished },
    mutate: jest.fn<void, [ISaveQueryMutationParams]>()
};

const mockFailedPublicationMutation: ISaveQueryResult = {
    isPending: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Failed },
    mutate: jest.fn<void, [ISaveQueryMutationParams]>()
};

const mockRequestErrorMutation: ISaveQueryResult = {
    isPending: false,
    isError: true,
    isSuccess: false,
    data: null,
    mutate: jest.fn<void, [ISaveQueryMutationParams]>()
};

const onCloseMock = jest.fn();



describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const dom = render(
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={false} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open with publication disabled', () => {
        const dom = render(
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open (draft)', () => {
        const dom = render(
                <SaveResultDialog mutation={mockSuccessDraftMutation} onClose={onCloseMock} open={true} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Failed publication status rendering test', () => {
    it('renders correctly when publication status is Failed', () => {
        const dom = render(
                <SaveResultDialog mutation={mockFailedPublicationMutation} onClose={onCloseMock} open={true} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Request error rendering test', () => {
    it('renders correctly when request isError is true', () => {
        const dom = render(
                <SaveResultDialog mutation={mockRequestErrorMutation} onClose={onCloseMock} open={true} />
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('invokes close function when cancel button is clicked', () => {
        render(
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
        );
        fireEvent.click(screen.getByText('saveResultDialog.ok'));
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });
});