import { CircularProgress, Container, DialogContent, Typography, Box } from '@mui/material';
import React from 'react';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';

const ProgressWrapper = styled(Container)`
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: 24px;
    gap: 16px;
`;

export const LoadingDialogContent: React.FC = () => {
    const { t } = useTranslation();

    return (
        <DialogContent dividers={true}>
            <ProgressWrapper>
                <CircularProgress size={48} />
                <Box textAlign="center">
                    <Typography variant="body1" gutterBottom>
                        {t('saveResultDialog.saving')}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        {t('saveResultDialog.pleaseWait')}
                    </Typography>
                </Box>
            </ProgressWrapper>
        </DialogContent>
    );
}

export default LoadingDialogContent;