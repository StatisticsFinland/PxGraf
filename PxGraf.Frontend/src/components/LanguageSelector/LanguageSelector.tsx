import { Stack, Button } from '@mui/material';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { LangText } from './LangText';
import styled from 'styled-components';
import InfoBubble from 'components/InfoBubble/InfoBubble';

const SelectorWrapper = styled(Stack)`
    padding: 8px;
`;

const StyledLangButton = styled(Button)<{ selected?: boolean }>`
    && {
        padding: 4px 10px;
        min-width: 0;
        text-transform: none;
        color: black;
        background-color: white;
        font-weight: ${({ selected }) => selected ? 700 : 400};
        border: ${({ selected }) => selected ? '1px solid black' : '1px solid transparent'};
        border-radius: 20px;
        &:hover {
            background-color: rgba(0, 0, 0, 0.06);
        }
    }
`;

export const LanguageSelector: React.FC = () => {
    const { t, i18n } = useTranslation();
    const { language, setLanguage, availableUiLanguages } = React.useContext(UiLanguageContext);

    return (
        <SelectorWrapper direction="row" alignItems="center" flexWrap='wrap'>
            {availableUiLanguages.map(lang => (
                <StyledLangButton size="small" href='#' selected={language === lang} aria-label={`${t('general.uiLanguage')}: ${i18n.getFixedT(lang)('lang.self')}`} key={lang} onClick={() => setLanguage(lang)}>
                    <LangText text={i18n.getFixedT(lang)('lang.self')} />
                </StyledLangButton>
            ))}
            <InfoBubble info={t("infoText.langSelector")} ariaLabel={t("general.uiLanguage")} />
        </SelectorWrapper>
    );
}

export default LanguageSelector;
