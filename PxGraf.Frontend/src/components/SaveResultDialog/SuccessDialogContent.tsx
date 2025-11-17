import { useTranslation } from 'react-i18next';
import {
    DialogContent, FormControl, IconButton, InputAdornment, InputLabel, OutlinedInput, Alert
} from '@mui/material';
import ContentPasteIcon from '@mui/icons-material/ContentPaste';
import styled from 'styled-components';
import React from 'react';
import { EQueryPublicationStatus } from "types/saveQuery";
import { MultiLanguageString } from "types/multiLanguageString";

const StyledOutlinedInput = styled(OutlinedInput)`
width: 380px;
`;

const StyledFormControl = styled(FormControl)`
margin-bottom: 16px;
`;

interface ISuccessDialogContentProps {
    queryId?: string;
    publicationStatus?: EQueryPublicationStatus;
    publicationMessage?: MultiLanguageString;
    isDraft?: boolean;
}

export const SuccessDialogContent: React.FC<ISuccessDialogContentProps> = ({
    queryId,
    publicationStatus,
    publicationMessage,
    isDraft = false
}) => {
    const { t, i18n } = useTranslation();
    const text = queryId || "Unknown";

    const getPublicationStatusMessage = () => {
        // If we have localized messages from webhook, use them
        if (publicationMessage && Object.keys(publicationMessage).length > 0) {
            const currentLanguage = i18n.language;

            // Try to get message for current language, fallback to any available language
            return publicationMessage[currentLanguage] ||
                publicationMessage[Object.keys(publicationMessage)[0]] ||
                t("saveResultDialog.publicationUnpublished");
        }

        return publicationStatus === EQueryPublicationStatus.Failed ? ("error.webhookResponseError") : t("saveResultDialog.webhookResponseSuccess");
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
            {!isDraft && publicationStatus !== undefined && publicationStatus !== EQueryPublicationStatus.Unpublished && (
                <Alert severity={getPublicationAlertSeverity()}>
                    {getPublicationStatusMessage()}
                </Alert>
            )}
        </DialogContent>
    );
}

export default SuccessDialogContent;