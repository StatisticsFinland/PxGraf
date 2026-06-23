import React from 'react';
import { Box, Button } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import { SaveContext } from 'contexts/saveContext';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import CellCount from 'components/CellCount/CellCount';

const FooterBtnWrapper = styled(Box)`
    grid-area: 'footer';
    display: flex;
    justify-content: flex-end;
    align-items: center;
    padding: 16px;
`;


interface IEditorFooterSectionProps {
    size?: number;
    maximumSize?: number;
    warningLimit?: number;
}

export const EditorFooterSection: React.FC<IEditorFooterSectionProps> = ({ size, maximumSize, warningLimit }) => {
    
    const { setSaveDialogOpen } = React.useContext(SaveContext);
    
    const { t } = useTranslation();
    
    return(
        <FooterBtnWrapper>
            {(size != null && maximumSize != null && warningLimit != null) ? <Box sx={{ marginRight: 'auto' }}><CellCount size={size} maximumSize={maximumSize} warningLimit={warningLimit} /></Box> : <></>}
            <InfoBubble info={t('infoText.save')} ariaLabel={t("editor.save")} />
            <Button variant="contained" size="small" startIcon={<SaveIcon />} onClick={() => setSaveDialogOpen(true)}>
                {t("editor.save")}
            </Button>
        </FooterBtnWrapper>
    );
}

export default EditorFooterSection;