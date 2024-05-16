import { useTranslation } from 'react-i18next';

import {
    DialogContent, FormControl, IconButton, InputAdornment, InputLabel, OutlinedInput
} from '@mui/material';

import ContentPasteIcon from '@mui/icons-material/ContentPaste';
import styled from 'styled-components';
import React from 'react';

const StyledOutlinedInput = styled(OutlinedInput)`
    width: 380px;
`;

interface ISuccessDialogContentProps {
    queryId?: string;
}

export const SuccessDialogContent: React.FC<ISuccessDialogContentProps> = ({ queryId }) => {

    const { t } = useTranslation();
    const text = queryId || "Unknown";

    return (
        <DialogContent dividers={true}>
            <FormControl>
                <InputLabel htmlFor="display-saved-query-id-input-label">{t("saveResultDialog.id")}</InputLabel>
                <StyledOutlinedInput
                    id="display-saved-query-id-input"
                    defaultValue={text}
                    inputProps={{ readOnly: true }}
                    label={t("saveResultDialog.id")}
                    endAdornment={
                        <InputAdornment position="end">
                            <IconButton
                                aria-label={t("saveResultDialog.copyToClipBoard")}
                                onClick={() => navigator.clipboard.writeText(text)}
                                edge="end">
                                <ContentPasteIcon />
                            </IconButton>
                        </InputAdornment>}
                />
            </FormControl>
        </DialogContent>
    );
}

export default SuccessDialogContent;