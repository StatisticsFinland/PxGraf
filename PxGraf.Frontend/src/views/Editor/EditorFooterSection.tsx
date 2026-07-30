import React from 'react';
import { Box, Button } from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import { SaveContext } from 'contexts/saveContext';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import CellCount from 'components/CellCount/CellCount';

const FooterBtnWrapper = styled(Box)(({ theme }) => ({
    gridArea: 'footer',
    height: 56,
    boxSizing: 'border-box',
    display: 'flex',
    justifyContent: 'flex-end',
    alignItems: 'center',
    padding: '8px 16px',
    borderTop: `1px solid ${theme.palette.divider}`,
    backgroundColor: theme.palette.background.paper,
}));

const CellCountWrapper = styled(Box)`
    height: 30px;
    display: flex;
    align-items: center;
    margin-right: auto;

    & .MuiAlert-root {
        min-height: 30px;
        box-sizing: border-box;
        align-items: center;
        padding-top: 0;
        padding-bottom: 0;
    }

    & .MuiAlert-icon,
    & .MuiAlert-message {
        padding-top: 4px;
        padding-bottom: 4px;
    }
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
            {(size != null && maximumSize != null && warningLimit != null) ? <CellCountWrapper><CellCount size={size} maximumSize={maximumSize} warningLimit={warningLimit} /></CellCountWrapper> : <></>}
            <InfoBubble info={t('infoText.save')} ariaLabel={t("editor.save")} />
            <Button variant="contained" size="small" startIcon={<SaveIcon />} sx={{ minWidth: 104 }} onClick={() => setSaveDialogOpen(true)}>
                {t("editor.save")}
            </Button>
        </FooterBtnWrapper>
    );
}

export default EditorFooterSection;