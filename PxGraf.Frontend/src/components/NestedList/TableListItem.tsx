import * as React from 'react';
import { Link } from "react-router-dom";
import { useTranslation } from 'react-i18next';

import { List, ListItem, ListItemIcon, ListItemText, ListItemButton, Divider, IconButton, Collapse, Tooltip } from '@mui/material';

import ExpandLess from '@mui/icons-material/ExpandLess';
import ExpandMore from '@mui/icons-material/ExpandMore';
import ListViewIcon from '@mui/icons-material/ListAltOutlined';

import { urls } from 'Router';
import NestedList from './NestedList';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { parseLanguageString } from 'utils/ApiHelpers';
import { IDatabaseGroupHeader } from 'types/tableListItems';

interface ITableListItemProps {
    currentPath: string[];
    item: IDatabaseGroupHeader;
    initialOpenState?: boolean;
    depth: number;
}

/**
 * Component used for displaying a database or subfolder in @see {@link NestedList} component.
 * @param {string[]} currentPath Path to the item in question in the Px file system.
 * @param {IDatabaseGroupHeader} item Response object that stores information about the database or subfolder.
 * @param {boolean} initialOpenState Whether the item should be displayed as opened.
 * @param {number} depth Current browsing depth.
 */
export const TableListItem: React.FC<ITableListItemProps> = ({ currentPath, item, initialOpenState, depth }) => {
    const { t } = useTranslation();
    const [isOpen, setIsOpen] = React.useState(initialOpenState ?? false);
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    return <React.Fragment key={`${item.code}-key`}>
        <ListItem
            secondaryAction={depth >= 1 ?
                <Tooltip title={t("tableSelect.listView")} aria-label={`${t("tableSelect.listView")}: ${item.name[displayLanguage]}`}>
                    <IconButton edge="end" component={Link} to={urls.tableList(currentPath)} >
                        <ListViewIcon />
                    </IconButton>
                </Tooltip> : <></>
            }>
            <ListItemButton sx={{ pl: depth * 4 }} onClick={() => setIsOpen(!isOpen)} aria-expanded={isOpen}>
                <ListItemIcon sx={{ minWidth: '32px' }}>
                    {isOpen ? <ExpandLess /> : <ExpandMore />}
                </ListItemIcon>
                {isOpen ? <ListItemText primary={<b>{`${item.name[displayLanguage]} ${parseLanguageString(item.languages)}`}</b>} /> : <ListItemText primary={`${item.name[displayLanguage]} ${parseLanguageString(item.languages)}`} />}
            </ListItemButton>
        </ListItem>
        <Divider />
        <Collapse in={isOpen} timeout="auto" unmountOnExit>
            <List component="div" disablePadding>
                <NestedList path={currentPath} depth={depth + 1} />
            </List>
        </Collapse>
    </React.Fragment>;
};