import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import SaveResultDialog from './SaveResultDialog';
import { EQueryPublicationStatus } from "types/saveQuery";
import { ISaveQueryResult } from 'api/services/queries';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';

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
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Success },
    mutate: function (_property: any): void {
        throw new Error('Function not implemented.');
    }
};

const mockSuccessDraftMutation: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Unpublished },
    mutate: function (_property: any): void {
        throw new Error('Function not implemented.');
    }
};

const mockErrorMutation: ISaveQueryResult = {
    isLoading: false,
    isError: false,
    isSuccess: true,
    data: { id: 'foobar', publicationStatus: EQueryPublicationStatus.Failed },
    mutate: function (_property: any): void {
        throw new Error('Function not implemented.');
    }
};

const onCloseMock = jest.fn();

const mockEditorContextPublicationTrue = {
    cubeQuery: null,
    setCubeQuery: jest.fn(),
    query: {},
    setQuery: jest.fn(),
    saveDialogOpen: false,
    setSaveDialogOpen: jest.fn(),
    selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
    setSelectedVisualizationUserInput: jest.fn(),
    visualizationSettingsUserInput: {},
    setVisualizationSettingsUserInput: jest.fn(),
    defaultSelectables: {},
    setDefaultSelectables: jest.fn(),
    loadedQueryId: '',
    setLoadedQueryId: jest.fn(),
    loadedQueryIsDraft: false,
    setLoadedQueryIsDraft: jest.fn(),
    publicationWebhookEnabled: true,
    setPublicationWebhookEnabled: jest.fn()
};



describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const dom = render(
            <EditorContext.Provider value={mockEditorContextPublicationTrue}>
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={false} />
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(
            <EditorContext.Provider value={mockEditorContextPublicationTrue}>
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open with publication disabled', () => {
        const mockEditorContextPublicationFalse = {
            ...mockEditorContextPublicationTrue,
            publicationWebhookEnabled: false,
            setPublicationWebhookEnabled: jest.fn()
        };

        const dom = render(
            <EditorContext.Provider value={mockEditorContextPublicationFalse}>
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open (draft)', () => {
        const dom = render(
            <EditorContext.Provider value={mockEditorContextPublicationTrue}>
                <SaveResultDialog mutation={mockSuccessDraftMutation} onClose={onCloseMock} open={true} />
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Error rendering test', () => {
    it('renders error correctly when open with error', () => {
        const dom = render(
            <EditorContext.Provider value={mockEditorContextPublicationTrue}>
                <SaveResultDialog mutation={mockErrorMutation} onClose={onCloseMock} open={true} />)
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('invokes close function when cancel button is clicked', () => {
        render(
            <EditorContext.Provider value={mockEditorContextPublicationTrue}>
                <SaveResultDialog mutation={mockSuccessMutation} onClose={onCloseMock} open={true} />
            </EditorContext.Provider>
        );
        fireEvent.click(screen.getByText('saveResultDialog.ok'));
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });
});