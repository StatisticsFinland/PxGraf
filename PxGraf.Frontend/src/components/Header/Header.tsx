import React, { useRef } from 'react';
import { useMediaQuery, Button, Box, Stack } from '@mui/material';
import { styled } from '@mui/material/styles';
import LanguageSelector from 'components/LanguageSelector/LanguageSelector';
import SavedQueryFinder from 'components/SavedQueryFinder/SavedQueryFinder';
import { useNavigationContext } from 'contexts/navigationContext';
import BreadcrumbNav from './BreadcrumbNav';
import logo from 'images/pxgraf-logo.png';
import logo_small from 'images/pxgraf-logo-small.png';
import { useTranslation } from 'react-i18next';
import { useLocation } from 'react-router-dom';
import { BasePath } from '../../envVars';

const Logo = styled('img')({
    display: 'block',
    height: 40,
});

const LogoLink = styled('a')({
    display: 'flex',
    alignItems: 'center',
    flexShrink: 0,
});

const HeaderWrapper = styled(Box)(({ theme }) => ({
  minHeight: 56,
  borderBottom: `1px solid ${theme.palette.divider}`,
  backgroundColor: theme.palette.background.paper,
}));

const LangSelectorWrapper = styled(Stack)({ width: '20%' });

const MenuRowWrapper = styled(Stack)({
  padding: '8px 16px',
  boxSizing: 'border-box',
  width: '100%',
  alignItems: 'center',
  justifyContent: 'flex-start',
    gap: 16,
});

const LinkWrapper = styled('div')({
  display: 'flex',
  flexDirection: 'row',
  justifyContent: 'flex-start',
  alignItems: 'center',
});

const BreadcrumbWrapper = styled('div')({ flex: 1, minWidth: 0 });

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

    const showBreadcrumb = location.pathname === '/' ||
        location.pathname.startsWith('/editor/') ||
        location.pathname.startsWith('/table-list/');

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
                <Button sx={{ position: 'absolute', left: '-9999px' }} href="#" onClick={(e) => { e.preventDefault(); focusOnContent(); }} ref={ref} disableFocusRipple>{t('general.contentLink')}</Button>
                <LogoLink href={indexUrl}><Logo alt={t('navbar.logoAlt')} src={isNarrowScreen ? logo_small : logo} /></LogoLink>
                <LinkWrapper>
                    <SavedQueryFinder oldQueryId={queryId} compact={isNarrowScreen} />
                </LinkWrapper>
                <BreadcrumbWrapper>
                    {showBreadcrumb && tablePath?.length > 0 && <BreadcrumbNav tablePath={tablePath} />}
                </BreadcrumbWrapper>
                <LangSelectorWrapper direction="row-reverse" marginLeft="auto">
                    <LanguageSelector />
                </LangSelectorWrapper>
            </MenuRowWrapper>
        </HeaderWrapper>
    );
}

export default Header;