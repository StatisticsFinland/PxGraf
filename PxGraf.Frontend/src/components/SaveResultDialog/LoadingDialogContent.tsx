import { CircularProgress, Container, DialogContent } from '@mui/material';
import React from 'react';
import styled from 'styled-components';

const ProgressWrapper = styled(Container)`
    display: flex;
    align-items: center;
    justify-content: center;
`;

export const LoadingDialogContent: React.FC = () => {
    return (
        <DialogContent dividers={true}>
            <ProgressWrapper>
                <CircularProgress />
            </ProgressWrapper>
        </DialogContent>
    );
}

export default LoadingDialogContent;