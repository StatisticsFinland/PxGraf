import * as React from 'react';
import { useTranslation } from 'react-i18next';
import { ListItem, ListItemIcon, ListItemText, Divider, Skeleton, Alert } from '@mui/material';
import UiLanguageContext from 'contexts/uiLanguageContext';
import styled from 'styled-components';
import { TableItem } from './TableItem';
import { TableListItem } from './TableListItem';
import useHierarchyParams from 'hooks/useHierarchyParams';
import useScrollToElement from 'hooks/useScrollToElement';
import { useTableQuery } from 'api/services/table';
import { IDatabaseGroupHeader, IDatabaseTable } from 'types/tableListItems';
import { sortDatabaseItems } from 'utils/sortingHelpers';

const StyledSkeleton = styled(Skeleton)`
  width: 24px;
  height: 24px;
`;

const StyledListItemText = styled(ListItemText)`
  margin-right: 32px;
`;

const ErrorAlert = styled(Alert)`
  width: 100%;
`;

/**
 * @property {string[]} path - Path to the current location in the tree.
 * @property {number} depth - Current browsing depth.
 */
interface INestedListProps {
    path: string[];
    depth?: number;
}

/**
 * Nested list component for displaying the database and table hierarchies.
 * @see TableListItem component is used for displaying databases and subfolders while @see {@link TableItem} component is used for displaying individual tables.
 */
export const NestedList: React.FC<INestedListProps> = ({ path, depth }) => {
    const { language } = React.useContext(UiLanguageContext);
    const { isLoading, isError, data } = useTableQuery(path);
    const { t } = useTranslation();
    
    const tablePath = useHierarchyParams();
    const activeId: string = tablePath.length && tablePath.join('-');
    const shouldScroll: boolean = activeId && data && path.length === tablePath.length - 1 && tablePath.join(',').startsWith(path.join(','));
    useScrollToElement(shouldScroll && activeId);

    const sortedGroups: IDatabaseGroupHeader[] = React.useMemo(() => {
        if (!data?.headers) return null;
        return sortDatabaseItems(data.headers, language);
    }, [data, language]);

    const sortedTables: IDatabaseTable[] = React.useMemo(() => {
        if (!data?.files) return null;
        return sortDatabaseItems(data.files, language);
    }, [data, language]);

    if (isLoading) {
        return <React.Fragment key={-1}>
            <ListItem>
                <ListItemIcon sx={{ ml: depth * 4 }}>
                    <StyledSkeleton variant="circular" />
                </ListItemIcon>
                <StyledListItemText primary={<Skeleton variant="text" />} />
            </ListItem>
            <Divider />
        </React.Fragment>;
    }
    else if (isError) {
        return <React.Fragment key={-2}>
            <ListItem>
                <ErrorAlert sx={{ pl: depth * 4 }} severity="error">{t("error.contentLoad")}</ErrorAlert>
            </ListItem>
            <Divider />
        </React.Fragment>;
    }

    return (
        <>
            {
                sortedGroups ? sortedGroups.map((item) => (
                    <TableListItem
                        key={`${item.code}-list-key`}
                        currentPath={[...path, item.code]}
                        item={item}
                        initialOpenState={sortedGroups.length < 2 || tablePath.join(',').startsWith([...path, item.code].join(','))}
                        depth={depth ?? 0}
                    />
                )) : <ErrorAlert sx={{ pl: depth * 4 }} severity="error">{t("error.contentLoad")}</ErrorAlert>
            }
            {
                sortedTables ? sortedTables.map((item) => (
                    <TableItem
                        key={`${item.fileName}-table-key`}
                        currentPath={[...path, item.fileName]}
                        item={item}
                        depth={depth ?? 0}
                    />
                )) : <ErrorAlert sx={{ pl: depth * 4 }} severity="error">{t("error.contentLoad")}</ErrorAlert>
            }
        </>
    );
}
export default NestedList;