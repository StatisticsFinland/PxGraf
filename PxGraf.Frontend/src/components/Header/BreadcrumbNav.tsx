import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Breadcrumbs, Typography } from '@mui/material';
import { useQueries } from '@tanstack/react-query';
import { tableQueryOptions } from 'api/services/table';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { useNavigationContext } from 'contexts/navigationContext';
import { useTranslation } from 'react-i18next';
import { urls } from 'Router';
import styled from 'styled-components';

const BreadcrumbLink = styled(Link)`
    color: inherit;
    text-decoration: none;
    &:hover {
        text-decoration: underline;
    }
`;

interface IBreadcrumbItem {
    type: 'directory' | 'table';
    label: string;
    to: string | null;
}

interface IBreadcrumbNavProps {
    tablePath: string[];
}

/**
 * Breadcrumb navigation showing the current table path with translated names as links.
 * Non-last segments link to the table tree with that level expanded.
 * The last segment links to the editor for the full path.
 */
const BreadcrumbNav: React.FC<IBreadcrumbNavProps> = ({ tablePath }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const { setTablePath } = useNavigationContext();
    const location = useLocation();
    const isOnEditor = location.pathname.startsWith('/editor');

    // Query parent paths for ALL segments to resolve translated names.
    // For segment at index i, we fetch tablePath.slice(0, i) to get the parent listing.
    const pathKey = tablePath.join(',');
    const queries = React.useMemo(
        () => tablePath.map((_, i) => tableQueryOptions(tablePath.slice(0, i))),
        // eslint-disable-next-line react-hooks/exhaustive-deps -- stable by pathKey string
        [pathKey]
    );

    const results = useQueries({ queries });

    const items: IBreadcrumbItem[] = tablePath.map((code, index) => {
        // Resolve translated label from parent listing; check both directories and tables
        let label = code;
        let type: 'directory' | 'table' = index === tablePath.length - 1 ? 'table' : 'directory';
        const result = results[index];
        if (result?.data) {
            const foundHeader = result.data.headers?.find(h => h.code === code);
            const foundFile = result.data.files?.find(f => f.fileName === code);
            if (foundHeader) {
                const displayLang = foundHeader.languages.includes(language) ? language : foundHeader.languages[0];
                label = foundHeader.name[displayLang] ?? code;
            } else if (foundFile) {
                type = 'table';
                const displayLang = foundFile.languages.includes(language) ? language : foundFile.languages[0];
                label = foundFile.name[displayLang] ?? code;
            }
        }

        // Tables link to the editor; directories link to the tree view.
        let to: string | null;
        if (type === 'table' && isOnEditor) {
            to = urls.editor(tablePath);
        } else if (type === 'table') {
            to = null; // current location, no link
        } else {
            to = `/?tablePath=${tablePath.slice(0, index + 1).join(',')}`;
        }

        return { type, label, to };
    });

    return (
        <Breadcrumbs
            separator=">"
            aria-label={t('header.tableBreadcrumb')}
            sx={{ '& .MuiBreadcrumbs-separator': { color: 'primary.main' } }}
        >
            {items.map((item, index) => (
                item.to ? (
                    <BreadcrumbLink
                        key={item.to}
                        to={item.to}
                        aria-current={item.type === 'table' ? 'page' : undefined}
                        state={item.type === 'table' ? { resetEditor: true } : undefined}
                        onClick={item.type === 'directory' ? () => setTablePath(tablePath.slice(0, index + 1)) : undefined}
                    >
                        <Typography
                            component="span"
                            color={item.type === 'table' ? 'text.primary' : 'primary'}
                        >
                            {item.label}
                        </Typography>
                    </BreadcrumbLink>
                ) : (
                    <Typography
                        key={`current-${index}`}
                        component="span"
                        color={item.type === 'table' ? 'text.primary' : 'primary'}
                    >
                        {item.label}
                    </Typography>
                )
            ))}
        </Breadcrumbs>
    );
};

export default BreadcrumbNav;
