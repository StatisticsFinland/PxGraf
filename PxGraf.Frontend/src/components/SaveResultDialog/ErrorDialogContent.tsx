import { useTranslation } from 'react-i18next';

import {
    Alert, DialogContent,
  } from '@mui/material';
import React from 'react';

export const ErrorDialogContent: React.FC = () => {
    const { t } = useTranslation();

    return (
        <DialogContent dividers={true}>
            <Alert severity="error"> {t('saveResultDialog.errorMessage')} </Alert>
        </DialogContent>
    );
}

export default ErrorDialogContent;