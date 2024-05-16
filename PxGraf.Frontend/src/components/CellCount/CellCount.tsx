import { useTranslation } from 'react-i18next';
import { Alert, AlertColor } from '@mui/material';
import React from 'react';

interface ICellCountProps {
    size: number;
    maximumSize: number;
    warningLimit: number;
}

export const CellCount: React.FC<ICellCountProps> = ({ size, maximumSize, warningLimit }) => {
    const { t } = useTranslation();
    let severity: AlertColor = "info";
    let textContent: string = t("cellCount.infoText");

    if (size > maximumSize) {
        severity = "error";
        textContent = t("cellCount.dangerText");
    } else if (size >= warningLimit) {
        severity = "warning";
        textContent = t("cellCount.warningText");
    }

    return (
        <div aria-live='polite'>
            <Alert severity={severity}>{`${textContent}: ${size}/${maximumSize}`}</Alert>
        </div>
    );
}

export default CellCount;