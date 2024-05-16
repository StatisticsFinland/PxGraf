import React, { useRef } from 'react';
import { useMediaQuery, Typography, Button, Box, Stack } from '@mui/material';
import styled from 'styled-components';
import LanguageSelector from 'components/LanguageSelector/LanguageSelector';
import SavedQueryFinder from 'components/SavedQueryFinder/SavedQueryFinder';
import { useNavigationContext } from 'contexts/navigationContext';
import logo from 'images/pxgraf-logo.png';
import logo_small from 'images/pxgraf-logo-small.png';
import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';

const Logo = styled.img`
  height: 60px;
  padding-right: 24px;

  @media (max-width: 800px) {
    padding-right: 24px;
  }
`;

const HeaderWrapper = styled(Box)`
  min-height: 75px;
`;

const LangSelectorWrapper = styled(Stack)`
  width: 20%;
`;

const MenuRowWrapper = styled(Stack)`
  padding: 8px;
  width: 100%;
  align-items: center;
  justify-content: flex-start;
`;

const LinkWrapper = styled.div`
  padding-left: 30px;
  display: flex;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  gap: 5px;
`;

const TitleWrapper = styled.div`
  padding-left: 8px;
  min-width: 20%;
`;

/**
 * Header component displayed on top of the page in all views.
 * Contains the logo, page title, language selector, and database selector link.
 */
const Header: React.FC = () => {
    const { t } = useTranslation();
    const location = useLocation();
    const {queryId}: {queryId: string} = location?.state ? location.state : {queryId: null};
    const isNarrowScreen = useMediaQuery('(max-width: 800px)');
    const { tablePath } = useNavigationContext();
    const headerRef = useRef(null);
  
    let indexUrl = '/';
    if(tablePath?.length) {
      indexUrl = indexUrl + `?tablePath=${tablePath.join(',')}`;
    }

    // Show different top title for editor and database/table selection
    const title = location.pathname?.split("/")[1] == "editor" ? t("general.editorTitle") : t("general.selectDatabaseTitle");

    const ref = React.useRef<HTMLAnchorElement>();

    React.useEffect(() => {
      if(ref.current) ref.current.focus();
    }, [ref?.current, location.pathname])

    const focusOnContent = () => {
        const mainContentElement = document.getElementById("mainContent");
        if (mainContentElement) {
            mainContentElement.focus();
        }
    };

    return (
        <HeaderWrapper>
            <MenuRowWrapper direction="row" ref={headerRef}>
                <Button sx={{ position: 'absolute', left: '-9999px' }} href="#" onClick={(e) => { e.preventDefault(); focusOnContent(); }} ref={ref}>{t('general.contentLink')}</Button>
                <a href={indexUrl}><Logo alt={t('navbar.logoAlt')} src={isNarrowScreen ? logo_small : logo} /></a>
                <TitleWrapper>
                    <Typography variant="h1">{title}</Typography>
                </TitleWrapper>
                <LinkWrapper>
                    <Button href={indexUrl}>{t('general.databaseSelectorLink')}</Button>
                    <SavedQueryFinder oldQueryId={queryId} />
                </LinkWrapper>
                <LangSelectorWrapper direction="row-reverse" marginLeft="auto">
                    <LanguageSelector />
                </LangSelectorWrapper>
            </MenuRowWrapper>
        </HeaderWrapper>
    );
}

export default Header;