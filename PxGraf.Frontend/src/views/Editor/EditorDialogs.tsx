import React from 'react';
import { UseMutationResult } from 'react-query';
import { SaveDialog } from 'components/SaveDialog/SaveDialog';
import { SaveResultDialog } from 'components/SaveResultDialog/SaveResultDialog';
import { EditorContext } from 'contexts/editorContext';
import { ISaveQueryResponse, ISaveQueryMutationParams } from 'api/services/queries';

interface IEditorDialogsProps {
    saveQueryMutation: UseMutationResult<ISaveQueryResponse, unknown, ISaveQueryMutationParams>;
}

export const EditorDialogs: React.FC<IEditorDialogsProps> = ({ saveQueryMutation }) => {
    const { setSaveDialogOpen, setLoadedQueryId, setLoadedQueryIsDraft } = React.useContext(EditorContext);
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