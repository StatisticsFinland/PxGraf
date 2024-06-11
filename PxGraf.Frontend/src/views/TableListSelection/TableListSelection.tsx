import { useParams } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import { List, ListItem, Divider, Container, Skeleton, Alert } from '@mui/material';

import { UiLanguageContext } from 'contexts/uiLanguageContext';
import React from "react";
import { DirectoryInfo } from 'components/DirectoryInfo/DirectoryInfo';
import TableInfo from "components/TableInfo/TableInfo";
import styled from "styled-components";
import { useTableQuery } from "api/services/table";
import { useLanguagesQuery } from "../../api/services/languages";
import { sortTableData } from 'utils/sortingHelpers';

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
    const databaseLanguages: string[] = useLanguagesQuery();
    let primaryLanguage: string;
    if (databaseLanguages) {
        primaryLanguage = databaseLanguages.includes(language) ? language : databaseLanguages[0];
    }

    const params = useParams();
    const path = params["*"].split("/").filter(p => p.length > 0);
    const { isLoading, isError, data } = useTableQuery(path, primaryLanguage);

    React.useEffect(() => {
        document.title = `${t("pages.tableList")} | PxGraf`;
    }, []);

    const sortedData = React.useMemo(() => {
        if (!data) return null;

        return sortTableData(data, primaryLanguage);
    }, [data, primaryLanguage, databaseLanguages]);

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
        content = sortedData.map((item) =>
            item.type === "t" ?
                <TableInfo path={params["*"]} item={item} key={`${item.id}-table-info`} /> :
                <DirectoryInfo path={params["*"]} item={item} key={`${item.id}-directory-info`} />
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