import { Stack, Button } from '@mui/material';
import { useTheme } from '@mui/material/styles';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { LangText } from './LangText';
import styled from 'styled-components';
import Typography from '@mui/material/Typography';
import InfoBubble from 'components/InfoBubble/InfoBubble';

const SelectorWrapper = styled(Stack)`
    padding: 8px;
`;

const StyledSecondaryText = styled(Typography)`
    margin-bottom: -2px;
    margin-top: -2px;
    text-align: right;
`;

const TitleWrapper = styled.div`
    display: flex;
    align-items: center;
`;

const StyledLangButton = styled(Button)`
    && {
        padding: 6px 12px;
        min-width: 0;
    }
`;

export const LanguageSelector: React.FC = () => {
    const { t } = useTranslation();
    const theme = useTheme();
    const { language, setLanguage, availableUiLanguages } = React.useContext(UiLanguageContext);

    return (
        <SelectorWrapper direction="column">
            <TitleWrapper>
                <StyledSecondaryText variant="body2" sx={{ color: theme.palette.text.secondary }}>
                    {t("general.uiLanguage")}
                </StyledSecondaryText>
                <InfoBubble info={t("infoText.langSelector")} ariaLabel={t("general.uiLanguage")} />
            </TitleWrapper>
            <Stack direction="row" flexWrap='wrap'>
                {availableUiLanguages.map(lang => (
                    <StyledLangButton size="small" href='#' aria-label={`${t('general.uiLanguage')}: ${t('lang.' + lang)}`} key={lang} onClick={() => setLanguage(lang)}>
                        <LangText text={lang.toUpperCase()} underline={language === lang} />
                    </StyledLangButton>
                ))}
            </Stack>
        </SelectorWrapper>
    );
}

export default LanguageSelector;
