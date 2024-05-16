import React from 'react';
import { Box, Button } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import { EditorContext } from 'contexts/editorContext';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import InfoBubble from 'components/InfoBubble/InfoBubble';

const FooterBtnWrapper = styled(Box)`
    grid-area: 'footer';
    display: flex;
    justify-content: flex-end;
    align-items: center;
    padding: 16px;
`;


export const EditorFooterSection: React.FC = () => {
    
    const { setSaveDialogOpen } = React.useContext(EditorContext);
    
    const { t } = useTranslation();
    
    return(
        <FooterBtnWrapper>
            <InfoBubble info={t('infoText.save')} ariaLabel={t("editor.save")} />
            <Button variant="contained" startIcon={<SaveIcon />} onClick={() => setSaveDialogOpen(true)}>
                {t("editor.save")}
            </Button>
        </FooterBtnWrapper>
    );
}

export default EditorFooterSection;