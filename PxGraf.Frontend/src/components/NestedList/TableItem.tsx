import { Divider, ListItem, ListItemButton, ListItemIcon, ListItemText } from "@mui/material";
import FileIcon from '@mui/icons-material/InsertDriveFileOutlined';
import { ITableListResponse } from "api/services/table";
import React from 'react';
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { urls } from "Router";
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { parseLanguageString } from "../../api/services/languages";

const StyledListItem = styled(ListItem)`
  background-color: #f8f8f8;
`;

interface ITableItemProps {
    currentPath: string[];
    item: ITableListResponse;
    depth: number;
}

/**
 * Component used for displaying a table in @see {@link NestedList} component.
 * @param {string[]} currentPath Path to the table in the Px file system
 * @param {ITableListResponse} item Response object that stores information about the database or subfolder.
 * @param {number} depth Current browsing depth.
 */
export const TableItem: React.FC<ITableItemProps> = ({ currentPath, item, depth }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    return <React.Fragment key={`${item.id}-key`}>
        <StyledListItem id={currentPath.join('-')}>
            <ListItemButton sx={{ mr: 3, pl: depth * 4 }} component={Link} to={urls.editor(currentPath)}>
                <ListItemIcon sx={{ minWidth: '32px' }}>
                    <FileIcon />
                </ListItemIcon>
                <ListItemText primary={`${item.text[displayLanguage]} ${parseLanguageString(item.languages)}`} secondary={t("tableSelect.updated") + ": " + new Date(item.updated).toLocaleString(language)} />
            </ListItemButton>
        </StyledListItem>
        <Divider />
    </React.Fragment>
}