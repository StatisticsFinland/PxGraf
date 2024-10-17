import { useParams } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import { List, ListItem, Divider, Container, Skeleton, Alert } from '@mui/material';
import React from 'react';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { DirectoryInfo } from 'components/DirectoryInfo/DirectoryInfo';
import TableInfo from "components/TableInfo/TableInfo";
import styled from "styled-components";
import { IDatabaseGroupHeader, IDatabaseTable, useTableQuery } from "api/services/table";
import { sortDatabaseGroups, sortDatabaseTables } from 'utils/sortingHelpers';

const TableQueryAlert = styled(Alert)`
  width: 100%;
`;

const StyledSkeleton = styled(Skeleton)`
  height: 128px;
  width: 100%;
`;

const TableListSelectionWrapper = styled(Container)`
  padding: 8px;
`;

/**
 * Component for displaying details of a table list subfolder's contents.
 */
export const TableListSelection: React.FC = () => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);

    const params = useParams();
    const path = params["*"].split("/").filter(p => p.length > 0);
    const { isLoading, isError, data } = useTableQuery(path);

    React.useEffect(() => {
        document.title = `${t("pages.tableList")} | PxGraf`;
    }, []);

    const sortedGroups: IDatabaseGroupHeader[] = React.useMemo(() => {
        if (!data?.headers) return null;

        return sortDatabaseGroups(data.headers, language);
    }, [data, language]);

    const sortedTables: IDatabaseTable[] = React.useMemo(() => {
        if (!data?.files) return null;

        return sortDatabaseTables(data.files, language);
    }, [data, language])

    let content: React.ReactNode;
    if (isError) {
        content =
            <>
                <ListItem>
                    <TableQueryAlert id="mainContent" severity="error">{t("error.contentLoad")}</TableQueryAlert>
                </ListItem>
                <Divider />
            </>

    }
    else if (isLoading) {
        content =
            <>
                <ListItem>
                    <StyledSkeleton variant="rectangular" />
                </ListItem>
                <Divider />
                <ListItem>
                    <StyledSkeleton variant="rectangular" />
                </ListItem>
                <Divider />
                <ListItem>
                    <StyledSkeleton variant="rectangular" />
                </ListItem>
                <Divider />
            </>
    }
    else {
        content = (
            <>
                {sortedGroups.map((item) => (
                    <DirectoryInfo path={params["*"]} item={item} key={`${item.code}-directory-info`} />
                ))}
                {sortedTables.map((item) => (
                    <TableInfo path={params["*"]} item={item} key={`${item.code}-table-info`} />
                ))}
            </>
        );
    }

    // actual rendered component
    return (
        <TableListSelectionWrapper maxWidth="md">
            <List sx={{ bgcolor: 'background.paper' }}>
                {content}
            </List>
        </TableListSelectionWrapper>
    );
}

export default TableListSelection;