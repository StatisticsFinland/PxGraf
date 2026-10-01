import React from 'react';
import { Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { AppVersion } from 'envVars';

const VersionDisplay: React.FC = () => {
    const { t } = useTranslation();

    return (
        <Typography component="p" variant="caption" color="text.secondary">
            {t('app.version', { version: AppVersion })}
        </Typography>
    );
};

export default VersionDisplay;