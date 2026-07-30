import { Stack, Button } from '@mui/material';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { LangText } from './LangText';
import { styled } from '@mui/material/styles';
import InfoBubble from 'components/InfoBubble/InfoBubble';

const SelectorWrapper = styled(Stack)({ padding: 4, gap: 2 });

const StyledLangButton = styled(Button, {
    shouldForwardProp: prop => prop !== 'selected',
})<{ selected?: boolean }>(({ selected, theme }) => ({
    padding: '4px 10px',
    minWidth: 0,
    color: selected ? theme.palette.primary.main : theme.palette.text.secondary,
    backgroundColor: 'transparent',
    fontWeight: selected ? theme.typography.fontWeightBold : theme.typography.fontWeightMedium,
    border: `1px solid ${selected ? theme.palette.primary.main : 'transparent'}`,
    borderRadius: theme.shape.borderRadius,
    '&:hover': {
        backgroundColor: theme.palette.primary.light,
    },
}));

export const LanguageSelector: React.FC = () => {
    const { t, i18n } = useTranslation();
    const { language, setLanguage, availableUiLanguages } = React.useContext(UiLanguageContext);

    return (
        <SelectorWrapper direction="row" alignItems="center" flexWrap='wrap'>
            {availableUiLanguages.map(lang => (
                <StyledLangButton size="small" selected={language === lang} aria-pressed={language === lang} aria-label={`${t('general.uiLanguage')}: ${i18n.getFixedT(lang)('lang.self')}`} key={lang} onClick={() => setLanguage(lang)}>
                    <LangText text={i18n.getFixedT(lang)('lang.self')} />
                </StyledLangButton>
            ))}
            <InfoBubble info={t("infoText.langSelector")} ariaLabel={t("general.uiLanguage")} />
        </SelectorWrapper>
    );
}

export default LanguageSelector;
