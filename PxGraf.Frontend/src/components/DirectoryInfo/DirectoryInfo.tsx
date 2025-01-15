import { Link } from "react-router-dom";
import { ListItemText, ListItemButton, Typography, Divider } from '@mui/material';
import { urls } from 'Router';
import React from 'react';
import { spacing } from 'utils/componentHelpers';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDatabaseGroupHeader } from 'types/tableListItems';

interface IDirectoryInfoProps {
    path: string;
    item: IDatabaseGroupHeader;
}

export const DirectoryInfo: React.FC<IDirectoryInfoProps> = ({ path, item }) => {
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    const currentPath = [...path, item.code];
    return (
        <>
            <ListItemButton id="mainContent" component={Link} to={urls.tableList(currentPath)}>
                <ListItemText primary={
                    <Typography variant="body1" sx={{ ...spacing(1) }}>
                        {item.name[displayLanguage]}
                    </Typography>
                } />
            </ListItemButton>
            <Divider />
        </>
    );
}
export default DirectoryInfo;