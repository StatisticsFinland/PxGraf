import React, { useRef } from 'react';
import { useMediaQuery, Button, Box, Stack } from '@mui/material';
import styled from 'styled-components';
import LanguageSelector from 'components/LanguageSelector/LanguageSelector';
import SavedQueryFinder from 'components/SavedQueryFinder/SavedQueryFinder';
import { useNavigationContext } from 'contexts/navigationContext';
import BreadcrumbNav from './BreadcrumbNav';
import logo from 'images/pxgraf-logo.png';
import logo_small from 'images/pxgraf-logo-small.png';
import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';
import { BasePath } from '../../envVars';

const Logo = styled.img`
  height: 40px;
  padding-right: 24px;

  @media (max-width: 800px) {
    padding-right: 24px;
  }
`;

const HeaderWrapper = styled(Box)`
  min-height: 56px;
`;

const LangSelectorWrapper = styled(Stack)`
  width: 20%;
`;

const MenuRowWrapper = styled(Stack)`
  padding: 4px;
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

const BreadcrumbWrapper = styled.div`
  padding-left: 16px;
  flex: 1;
`;

/**
 * Header component displayed on top of the page in all views.
 * Contains the logo, table path breadcrumb, language selector, and saved query finder.
 */
const Header: React.FC = () => {
    const { t } = useTranslation();
    const location = useLocation();
    const {queryId}: {queryId: string} = location?.state ? location.state : {queryId: null};
    const isNarrowScreen = useMediaQuery('(max-width: 800px)');
    const { tablePath } = useNavigationContext();
    const headerRef = useRef(null);

    let indexUrl: string = BasePath || '/';
    if (tablePath?.length) {
        indexUrl = `${BasePath}/?tablePath=${tablePath.join(',')}`;
    }

    const ref = React.useRef<HTMLAnchorElement>(null);

    React.useEffect(() => {
      if(ref.current) ref.current.focus();
    }, [location.pathname]);

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
                <BreadcrumbWrapper>
                    {tablePath?.length > 0 && <BreadcrumbNav tablePath={tablePath} />}
                </BreadcrumbWrapper>
                <LinkWrapper>
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