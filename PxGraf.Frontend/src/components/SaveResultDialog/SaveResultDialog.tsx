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
}

interface ISaveDialogContentProps {
    result: ISaveQueryResult;
}

export const SaveResultDialog: React.FC<ISaveResultDialogProps> = ({ open, onClose, mutation}) => {
    
    const { t } = useTranslation();
    
    const HeaderText = () => {
        if(mutation.isLoading) { return t('saveDialog.saveQuery') }
        if(mutation.isError) { return t('saveResultDialog.fail') }
        if(mutation.isSuccess) { return t('saveResultDialog.success') }
    }

    return (
        <Dialog
        open={open}
        onClose={onClose}
        scroll='paper'
        aria-labelledby="scroll-dialog-title"
        aria-describedby="scroll-dialog-description"
        >
            <DialogTitle id="result-dialog-title">{ HeaderText() }</DialogTitle>
            <SaveDialogContent result={mutation}/>
            <DialogActions>
                <Button onClick={onClose} variant="contained">{t("saveResultDialog.ok")}</Button>
            </DialogActions>
        </Dialog>
    );
}

const SaveDialogContent: React.FC<ISaveDialogContentProps> = ({result}) => {
    if(result.isLoading) { return <LoadingDialogContent/> }
    else if(result.isSuccess) { return <SuccessDialogContent queryId = {result.data?.id}/>; }
    else { return <ErrorDialogContent/> }
}

export default SaveResultDialog;