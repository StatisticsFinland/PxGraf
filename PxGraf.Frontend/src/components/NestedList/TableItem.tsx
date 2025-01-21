import { Alert, AlertTitle, Divider, ListItem, ListItemButton, ListItemIcon, ListItemText } from "@mui/material";
import FileIcon from '@mui/icons-material/InsertDriveFileOutlined';
import React from 'react';
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { urls } from "Router";
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { parseLanguageString } from 'utils/ApiHelpers';
import { IDatabaseTable } from 'types/tableListItems';

const StyledListItem = styled(ListItem)`
  background-color: #f8f8f8;
`;

const ErrorAlert = styled(Alert)`
  width: 100%;
`;

/**
 * @property {string[]} currentPath Path to the table in in the hierarchy of the table list.
 * @property {IDatabaseTable} item Contains information about the table.
 * @property {number} depth Current depth in the hierarchy of the table list.
 */
interface ITableItemProps {
    currentPath: string[];
    item: IDatabaseTable;
    depth: number;
}

/**
 * Component used for displaying a table in @see {@link NestedList} component.
 */
export const TableItem: React.FC<ITableItemProps> = ({ currentPath, item, depth }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    return <React.Fragment key={`${item.fileName}-key`}>
        <StyledListItem id={currentPath.join('-')}>
            <ListItemButton sx={{ mr: 3, pl: depth * 4 }} component={Link} to={urls.editor(currentPath)}>
                {item.error ?
                    <ErrorAlert sx={{ pl: depth * 4 }} severity="warning">
                        <AlertTitle>{`${item.name[displayLanguage] ?? item.fileName}`}</AlertTitle>
                        {t("error.contentVariableMissing")}
                    </ErrorAlert>
                    :
                    <React.Fragment>
                        <ListItemIcon sx={{ minWidth: '32px' }}>
                            <FileIcon />
                        </ListItemIcon>
                        <ListItemText primary={`${item.name[displayLanguage]} ${parseLanguageString(item.languages)}`} secondary={t("tableSelect.updated") + ": " + new Date(item.lastUpdated).toLocaleString(language)} />
                    </React.Fragment>
                }
            </ListItemButton>
        </StyledListItem>
        <Divider />
    </React.Fragment>
}