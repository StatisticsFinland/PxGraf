import React from 'react';
import { Alert, AlertTitle, Box, Button } from '@mui/material';
import i18n from 'i18next';

interface IErrorBoundaryProps {
    children: React.ReactNode;
}

interface IErrorBoundaryState {
    hasError: boolean;
}

class ErrorBoundary extends React.Component<IErrorBoundaryProps, IErrorBoundaryState> {
    constructor(props: IErrorBoundaryProps) {
        super(props);
        this.state = { hasError: false };
    }

    static getDerivedStateFromError(): IErrorBoundaryState {
        return { hasError: true };
    }

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo): void {
        console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    handleReset = () => {
        this.setState({ hasError: false });
    };

    render() {
        if (this.state.hasError) {
            return (
                <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', p: 3 }}>
                    <Alert severity="error" sx={{ maxWidth: 600 }}>
                        <AlertTitle>{i18n.t('error.boundaryTitle')}</AlertTitle>
                        {i18n.t('error.boundaryMessage')}
                        <Box sx={{ mt: 2 }}>
                            <Button variant="contained" onClick={this.handleReset}>
                                {i18n.t('error.boundaryRetry')}
                            </Button>
                        </Box>
                    </Alert>
                </Box>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;
