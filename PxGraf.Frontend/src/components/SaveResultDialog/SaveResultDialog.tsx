import { useTranslation } from 'react-i18next';
import { Button, Dialog, DialogTitle, DialogActions } from '@mui/material';
import { SuccessDialogContent } from './SuccessDialogContent';
import { ErrorDialogContent } from './ErrorDialogContent';
import { LoadingDialogContent } from './LoadingDialogContent';
import React from 'react';
import { ISaveQueryResult } from 'api/services/queries';

interface ISaveResultDialogProps {
    open: boolean;
    onClose: () => void;
    mutation: ISaveQueryResult;
    isDraft?: boolean;
}

interface ISaveDialogContentProps {
    result: ISaveQueryResult;
    isDraft?: boolean;
}

export const SaveResultDialog: React.FC<ISaveResultDialogProps> = ({
    open,
    onClose,
    mutation,
    isDraft = false
}) => {

    const { t } = useTranslation();

    const HeaderText = () => {
        if (mutation.isLoading) { return t('saveDialog.saveQuery') }
        if (mutation.isError) { return t('saveResultDialog.fail') }
        if (mutation.isSuccess) { return t('saveResultDialog.success') }
    }

    const handleClose = () => {
        // Prevent closing while loading
        if (mutation.isLoading) {
            return;
        }
        onClose();
    };

    const canClose = !mutation.isLoading;

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            disableEscapeKeyDown={mutation.isLoading}
            scroll='paper'
            aria-labelledby="scroll-dialog-title"
            aria-describedby="scroll-dialog-description"
        >
            <DialogTitle id="result-dialog-title">{HeaderText()}</DialogTitle>
            <SaveDialogContent result={mutation} isDraft={isDraft} />
            <DialogActions>
                <Button
                    onClick={handleClose}
                    variant="contained"
                    disabled={!canClose}
                >
                    {t("saveResultDialog.ok")}
                </Button>
            </DialogActions>
        </Dialog>
    );
}

const SaveDialogContent: React.FC<ISaveDialogContentProps> = ({ result, isDraft }) => {
    if (result.isLoading) { return <LoadingDialogContent /> }
    else if (result.isSuccess) {
        return (
            <SuccessDialogContent
                queryId={result.data?.id}
                publicationStatus={result.data?.publicationStatus}
                publicationMessage={result.data?.publicationMessage}
                isDraft={isDraft}
            />
        );
    }
    else { return <ErrorDialogContent /> }
}

export default SaveResultDialog;