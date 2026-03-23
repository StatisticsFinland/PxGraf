import * as React from 'react';

interface ISaveContext {
    saveDialogOpen: boolean;
    setSaveDialogOpen: React.Dispatch<React.SetStateAction<boolean>>;
    loadedQueryId: string;
    setLoadedQueryId: React.Dispatch<React.SetStateAction<string>>;
    loadedQueryIsDraft: boolean;
    setLoadedQueryIsDraft: React.Dispatch<React.SetStateAction<boolean>>;
    publicationWebhookEnabled: boolean;
    setPublicationWebhookEnabled: React.Dispatch<React.SetStateAction<boolean>>;
}

export const SaveContext = React.createContext<ISaveContext>({
    saveDialogOpen: false,
    setSaveDialogOpen: () => { /* no base implementation */ },
    loadedQueryId: '',
    setLoadedQueryId: () => { /* no base implementation */ },
    loadedQueryIsDraft: false,
    setLoadedQueryIsDraft: () => { /* no base implementation */ },
    publicationWebhookEnabled: true,
    setPublicationWebhookEnabled: () => { /* no base implementation */ },
});

export const SaveProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [saveDialogOpen, setSaveDialogOpen] = React.useState(false);
    const [loadedQueryId, setLoadedQueryId] = React.useState('');
    const [loadedQueryIsDraft, setLoadedQueryIsDraft] = React.useState(false);
    const [publicationWebhookEnabled, setPublicationWebhookEnabled] = React.useState(true);

    const contextValue = React.useMemo(() => ({
        saveDialogOpen, setSaveDialogOpen,
        loadedQueryId, setLoadedQueryId,
        loadedQueryIsDraft, setLoadedQueryIsDraft,
        publicationWebhookEnabled, setPublicationWebhookEnabled,
    }), [saveDialogOpen, setSaveDialogOpen, loadedQueryId, setLoadedQueryId, loadedQueryIsDraft, setLoadedQueryIsDraft, publicationWebhookEnabled, setPublicationWebhookEnabled]);

    return (
        <SaveContext.Provider value={contextValue}>
            {children}
        </SaveContext.Provider>
    );
};
