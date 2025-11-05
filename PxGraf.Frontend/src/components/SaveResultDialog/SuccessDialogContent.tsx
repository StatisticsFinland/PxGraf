import { useTranslation } from 'react-i18next';
import {
    DialogContent, FormControl, IconButton, InputAdornment, InputLabel, OutlinedInput, Alert
} from '@mui/material';
import ContentPasteIcon from '@mui/icons-material/ContentPaste';
import styled from 'styled-components';
import React from 'react';
import { EQueryPublicationStatus } from 'api/services/queries';

const StyledOutlinedInput = styled(OutlinedInput)`
    width: 380px;
`;

const StyledFormControl = styled(FormControl)`
    margin-bottom: 16px;
`;

interface ISuccessDialogContentProps {
    queryId?: string;
    publicationStatus?: EQueryPublicationStatus;
    isDraft?: boolean;
}

export const SuccessDialogContent: React.FC<ISuccessDialogContentProps> = ({
    queryId,
    publicationStatus,
    isDraft = false
}) => {
    const { t } = useTranslation();
    const text = queryId || "Unknown";

    const getPublicationStatusMessage = () => {
        switch (publicationStatus) {
            case EQueryPublicationStatus.Success:
                return t("saveResultDialog.publicationSuccess");
            case EQueryPublicationStatus.Failed:
                return t("saveResultDialog.publicationFailed");
            case EQueryPublicationStatus.Unpublished:
                return t("saveResultDialog.publicationUnpublished");
            default:
                return t("saveResultDialog.publicationUnpublished");
        }
    };

    const getPublicationAlertSeverity = () => {
        switch (publicationStatus) {
            case EQueryPublicationStatus.Success:
                return "success";
            case EQueryPublicationStatus.Failed:
                return "error";
            default:
                return "info";
        }
    };

    return (
        <DialogContent dividers={true}>
            <StyledFormControl>
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
            </StyledFormControl>
            {!isDraft && publicationStatus && (
                <Alert severity={getPublicationAlertSeverity()}>
                    <strong>{t("saveResultDialog.publicationStatus")}:</strong> {getPublicationStatusMessage()}
                </Alert>
            )}
        </DialogContent>
    );
}

export default SuccessDialogContent;