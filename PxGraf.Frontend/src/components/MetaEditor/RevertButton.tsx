import React from 'react';
import { useTranslation } from 'react-i18next';

import { IconButton, Tooltip} from '@mui/material';
import UndoIcon from '@mui/icons-material/Undo';

interface IRevertButtonProps {
    onClick: (value?: string) => void;
}

export const RevertButton: React.FC<IRevertButtonProps> = ({ onClick }) => {
    const { t } = useTranslation();

    return (
        <Tooltip title={t("editMetadata.discardChanges")}>
            <span>{/* span wrapper is needed to tooltip work when IconButton is disabled */}
                <IconButton
                    aria-label={t("editMetadata.discardChanges")}
                    onClick={() => onClick()}
                    edge="end"
                >
                    <UndoIcon />
                </IconButton>
            </span>
        </Tooltip>
    );
}

export default RevertButton;