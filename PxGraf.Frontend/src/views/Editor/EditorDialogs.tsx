import React from 'react';
import { UseMutationResult } from '@tanstack/react-query';
import { SaveDialog } from 'components/SaveDialog/SaveDialog';
import { SaveResultDialog } from 'components/SaveResultDialog/SaveResultDialog';
import { SaveContext } from 'contexts/saveContext';
import { ISaveQueryResponse, ISaveQueryMutationParams } from 'api/services/queries';
import MetadataDialog from 'components/MetaEditor/MetadataDialog';
import { IDimension } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';

interface IEditorDialogsProps {
    saveQueryMutation: UseMutationResult<ISaveQueryResponse, unknown, ISaveQueryMutationParams>;
    metadataDialogOpen: boolean;
    metadataDimensions: IDimension[];
    contentLanguages: string[];
    metadataLanguage: string;
    cubeQuery: ICubeQuery;
    onMetadataApply: (variableQueries: ICubeQuery['variableQueries']) => void;
    onMetadataClose: () => void;
}

export const EditorDialogs: React.FC<IEditorDialogsProps> = ({
    saveQueryMutation,
    metadataDialogOpen,
    metadataDimensions,
    contentLanguages,
    metadataLanguage,
    cubeQuery,
    onMetadataApply,
    onMetadataClose,
}) => {
    const { setSaveDialogOpen, setLoadedQueryId, setLoadedQueryIsDraft } = React.useContext(SaveContext);
    const [saveResultDialogOpen, setSaveResultDialogOpen] = React.useState(false);
    const [lastSavedAsDraft, setLastSavedAsDraft] = React.useState(false);

    /* istanbul ignore next */
    const saveQueryAndShowResult = (archive: boolean, isDraft: boolean) => {
        setLastSavedAsDraft(isDraft);
        setSaveDialogOpen(false);
        setSaveResultDialogOpen(true);
        saveQueryMutation.mutate(
            { archive, isDraft },
            {
                onSuccess: (data) => {
                    setLoadedQueryId(data.id);
                    setLoadedQueryIsDraft(isDraft);
                }
            }
        );
    }

    return (
        <>
            <MetadataDialog
                open={metadataDialogOpen}
                dimensions={metadataDimensions}
                contentLanguages={contentLanguages}
                initialLanguage={metadataLanguage}
                initialCubeQuery={cubeQuery}
                onApply={onMetadataApply}
                onClose={onMetadataClose}
            />
            <SaveDialog onSave={saveQueryAndShowResult} />
            <SaveResultDialog
                open={saveResultDialogOpen}
                onClose={() => setSaveResultDialogOpen(false)}
                mutation={saveQueryMutation}
                isDraft={lastSavedAsDraft}
            />
        </>
    );
}

export default EditorDialogs;