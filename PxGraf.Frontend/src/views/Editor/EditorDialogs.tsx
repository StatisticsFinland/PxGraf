import React from 'react';

import { SaveDialog } from 'components/SaveDialog/SaveDialog';
import { SaveResultDialog } from 'components/SaveResultDialog/SaveResultDialog';
import { EditorContext } from 'contexts/editorContext';
import { ISaveQueryResult } from 'api/services/queries';

interface IEditorDialogsProps {
    saveQueryMutation: ISaveQueryResult;
}

export const EditorDialogs: React.FC<IEditorDialogsProps> = ({ saveQueryMutation }) => {

    /* istanbul ignore next */
    const saveQueryAndShowResult = (archive: boolean, isDraft: boolean) => {
        saveQueryMutation.mutate({ archive, isDraft });
        setSaveDialogOpen(false);
        setSaveResultDialogOpen(true);
    }

    const { setSaveDialogOpen } = React.useContext(EditorContext);
    const [saveResultDialogOpen, setSaveResultDialogOpen] = React.useState(false);
    
    return (
        <>
            <SaveDialog onSave={saveQueryAndShowResult} />
            <SaveResultDialog
                open={saveResultDialogOpen}
                onClose={() => setSaveResultDialogOpen(false)}
                mutation={saveQueryMutation}
            />
        </>
    );
}

export default EditorDialogs;