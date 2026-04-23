import React from 'react';
import { SaveContext, SaveProvider } from 'contexts/saveContext';
import { act, render } from '@testing-library/react';
import '@testing-library/jest-dom';

const TestComponent = () => {
    const {
        saveDialogOpen, setSaveDialogOpen,
        loadedQueryId, setLoadedQueryId,
        loadedQueryIsDraft, setLoadedQueryIsDraft,
        publicationWebhookEnabled, setPublicationWebhookEnabled,
    } = React.useContext(SaveContext);

    return (
        <>
            <div data-testid="dialogOpen">{String(saveDialogOpen)}</div>
            <div data-testid="queryId">{loadedQueryId || 'empty'}</div>
            <div data-testid="isDraft">{String(loadedQueryIsDraft)}</div>
            <div data-testid="webhookEnabled">{String(publicationWebhookEnabled)}</div>
            <button data-testid="openDialog" onClick={() => setSaveDialogOpen(true)} />
            <button data-testid="setQueryId" onClick={() => setLoadedQueryId('query-123')} />
            <button data-testid="setDraft" onClick={() => setLoadedQueryIsDraft(true)} />
            <button data-testid="disableWebhook" onClick={() => setPublicationWebhookEnabled(false)} />
        </>
    );
};

describe('SaveContext', () => {
    it('should have correct default values', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );
        expect(getByTestId('dialogOpen')).toHaveTextContent('false');
        expect(getByTestId('queryId')).toHaveTextContent('empty');
        expect(getByTestId('isDraft')).toHaveTextContent('false');
        expect(getByTestId('webhookEnabled')).toHaveTextContent('true');
    });

    it('should update saveDialogOpen', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );

        act(() => {
            getByTestId('openDialog').click();
        });

        expect(getByTestId('dialogOpen')).toHaveTextContent('true');
    });

    it('should update loadedQueryId', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );

        act(() => {
            getByTestId('setQueryId').click();
        });

        expect(getByTestId('queryId')).toHaveTextContent('query-123');
    });

    it('should update loadedQueryIsDraft', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );

        act(() => {
            getByTestId('setDraft').click();
        });

        expect(getByTestId('isDraft')).toHaveTextContent('true');
    });

    it('should update publicationWebhookEnabled', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );

        act(() => {
            getByTestId('disableWebhook').click();
        });

        expect(getByTestId('webhookEnabled')).toHaveTextContent('false');
    });

    it('should handle multiple state updates independently', () => {
        const { getByTestId } = render(
            <SaveProvider><TestComponent /></SaveProvider>
        );

        act(() => {
            getByTestId('openDialog').click();
            getByTestId('setQueryId').click();
        });

        expect(getByTestId('dialogOpen')).toHaveTextContent('true');
        expect(getByTestId('queryId')).toHaveTextContent('query-123');
        // Unchanged values remain at defaults
        expect(getByTestId('isDraft')).toHaveTextContent('false');
        expect(getByTestId('webhookEnabled')).toHaveTextContent('true');
    });
});
