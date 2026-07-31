import { Alert, AlertTitle, Divider, ListItem, ListItemButton, ListItemIcon, ListItemText } from "@mui/material";
import FileIcon from '@mui/icons-material/InsertDriveFileOutlined';
import React from 'react';
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { styled } from '@mui/material/styles';
import { urls } from 'routes/urls';
import { UiLanguageContext } from "contexts/uiLanguageContext";
import { parseLanguageString } from 'utils/ApiHelpers';
import { IDatabaseTable } from 'types/tableListItems';
import { getErrorText } from "../../utils/editorHelpers";

const StyledListItem = styled(ListItem)({
    padding: 0,
});

const TableButton = styled(ListItemButton)<{ component?: React.ElementType; to?: string }>(({ theme }) => ({
    minHeight: 56,
    paddingTop: theme.spacing(0.75),
    paddingBottom: theme.spacing(0.75),
    paddingRight: theme.spacing(2),
    backgroundColor: theme.palette.background.paper,
    borderLeft: '3px solid transparent',
    '&:hover': {
        backgroundColor: theme.palette.action.hover,
    },
    '&.Mui-focusVisible': {
        outline: `2px solid ${theme.palette.primary.main}`,
        outlineOffset: -2,
    },
}));

const TableMetadata = styled('span')(({ theme }) => ({
    display: 'flex',
    flexWrap: 'wrap',
    columnGap: theme.spacing(1.5),
    color: theme.palette.text.secondary,
    fontSize: theme.typography.caption.fontSize,
    lineHeight: 1.35,
    '& > span + span::before': {
        content: '"\\2022"',
        marginRight: theme.spacing(1.5),
        color: theme.palette.divider,
    },
}));

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
    const displayName = item.name[displayLanguage] ?? Object.values(item.name)[0] ?? item.fileName;

    return <React.Fragment key={`${item.fileName}-key`}>
        <StyledListItem id={currentPath.join('-')}>
            <TableButton sx={{ pl: 2.5 + depth * 2.5 }} component={Link} to={urls.editor(currentPath)}>
                {item.error ?
                    <ErrorAlert severity="warning">
                        <AlertTitle>{displayName}</AlertTitle>
                        {getErrorText(item.error, t)}
                    </ErrorAlert>
                    :
                    <React.Fragment>
                        <ListItemIcon sx={{ minWidth: '32px' }}>
                            <FileIcon />
                        </ListItemIcon>
                        <ListItemText
                            primary={displayName}
                            secondary={
                                <TableMetadata>
                                    <span>{parseLanguageString(item.languages)}</span>
                                    <span>{t("tableSelect.updated") + ": " + new Date(item.lastUpdated).toLocaleString(language)}</span>
                                </TableMetadata>
                            }
                            slotProps={{ primary: { sx: { overflowWrap: 'anywhere' } } }}
                        />
                    </React.Fragment>
                }
            </TableButton>
        </StyledListItem>
        <Divider />
    </React.Fragment>
}