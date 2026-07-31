import * as React from 'react';

import { Collapse, Divider, List, ListItem, ListItemButton, ListItemIcon, ListItemText } from '@mui/material';
import { styled } from '@mui/material/styles';

import ChevronRight from '@mui/icons-material/ChevronRight';
import FolderOutlined from '@mui/icons-material/FolderOutlined';

import NestedList from './NestedList';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { parseLanguageString } from 'utils/ApiHelpers';
import { IDatabaseGroupHeader } from 'types/tableListItems';

const FolderListItem = styled(ListItem)({
    padding: 0,
});

const FolderButton = styled(ListItemButton, {
    shouldForwardProp: (prop) => prop !== 'depth',
})<{ depth: number }>(({ theme, depth }) => ({
    minHeight: 52,
    paddingTop: theme.spacing(0.5),
    paddingBottom: theme.spacing(0.5),
    paddingLeft: theme.spacing(1.5 + depth * 2.5),
    paddingRight: theme.spacing(2),
    position: 'relative',
    backgroundColor: 'transparent',
    transition: 'background-color 120ms ease',
    '&::before': {
        content: '""',
        position: 'absolute',
        top: 0,
        bottom: 0,
        left: theme.spacing(depth * 2.5),
        width: 3,
        backgroundColor: 'transparent',
    },
    '&[aria-expanded="true"]': {
        backgroundColor: theme.palette.action.selected,
        '&::before': {
            backgroundColor: theme.palette.primary.main,
        },
    },
    '&:hover': {
        backgroundColor: theme.palette.action.hover,
    },
    '&.Mui-focusVisible': {
        outline: `2px solid ${theme.palette.primary.main}`,
        outlineOffset: -2,
    },
    '@media (prefers-reduced-motion: reduce)': {
        transition: 'none',
    },
}));

const Chevron = styled(ChevronRight, {
    shouldForwardProp: (prop) => prop !== 'open',
})<{ open: boolean }>(({ open }) => ({
    transform: open ? 'rotate(90deg)' : 'rotate(0deg)',
    transition: 'transform 140ms ease',
    '@media (prefers-reduced-motion: reduce)': {
        transition: 'none',
    },
}));

const TreeCollapse = styled(Collapse)({
    '@media (prefers-reduced-motion: reduce)': {
        transitionDuration: '0ms !important',
    },
});

const LanguageMetadata = styled('span')(({ theme }) => ({
    color: theme.palette.text.secondary,
    fontSize: theme.typography.caption.fontSize,
    lineHeight: 1.35,
}));

interface ITableListItemProps {
    currentPath: string[];
    item: IDatabaseGroupHeader;
    initialOpenState?: boolean;
    depth: number;
    onPathOpen?: (path: string[]) => void;
}

/**
 * Component used for displaying a database or subfolder in @see {@link NestedList} component.
 * @param {string[]} currentPath Path to the item in question in the Px file system.
 * @param {IDatabaseGroupHeader} item Response object that stores information about the database or subfolder.
 * @param {boolean} initialOpenState Whether the item should be displayed as opened.
 * @param {number} depth Current browsing depth.
 */
export const TableListItem: React.FC<ITableListItemProps> = ({ currentPath, item, initialOpenState, depth, onPathOpen }) => {
    const [isOpen, setIsOpen] = React.useState(initialOpenState ?? false);
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];
    const displayName = item.name[displayLanguage] ?? Object.values(item.name)[0] ?? item.code;

    const handleToggle = () => {
        const newIsOpen = !isOpen;
        setIsOpen(newIsOpen);
        if (newIsOpen) {
            onPathOpen?.(currentPath);
        }
    };

    return <React.Fragment key={`${item.code}-key`}>
        <FolderListItem id={currentPath.join('-')}>
            <FolderButton depth={depth} onClick={handleToggle} aria-expanded={isOpen}>
                <ListItemIcon sx={{ minWidth: 60, color: isOpen ? 'primary.main' : 'text.secondary' }}>
                    <Chevron open={isOpen} aria-hidden="true" />
                    <FolderOutlined sx={{ ml: 0.5 }} aria-hidden="true" />
                </ListItemIcon>
                <ListItemText
                    primary={displayName}
                    secondary={<LanguageMetadata>{parseLanguageString(item.languages)}</LanguageMetadata>}
                    slotProps={{ primary: { sx: { fontWeight: isOpen ? 600 : 400 } } }}
                />
            </FolderButton>
        </FolderListItem>
        <Divider />
        <TreeCollapse in={isOpen} timeout={140} unmountOnExit>
            <List component="div" disablePadding>
                <NestedList path={currentPath} depth={depth + 1} onPathOpen={onPathOpen} />
            </List>
        </TreeCollapse>
    </React.Fragment>;
};