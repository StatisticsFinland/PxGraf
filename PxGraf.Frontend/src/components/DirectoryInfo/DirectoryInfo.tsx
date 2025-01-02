import { Link } from "react-router-dom";
import { ListItemText, ListItemButton, Typography, Divider } from '@mui/material';
import { urls } from 'Router';
import React from 'react';
import { spacing } from 'utils/componentHelpers';
import { MultiLanguageString } from "../../types/multiLanguageString";
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IDirectoryInfoProps {
    path: string;
    item: {
        code?: string;
        lastUpdated?: string;
        name?: MultiLanguageString;
        languages?: string[];
    }
}

export const DirectoryInfo: React.FC<IDirectoryInfoProps> = ({ path, item }) => {
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    const currentPath = [...path, item.code];
    return (
        <>
            <ListItemButton id="mainContent" component={Link} to={urls.tableList(currentPath)}>
                <ListItemText primary={
                    <>
                        <Typography variant="body1" sx={{ ...spacing(1) }}>
                            {item.name[displayLanguage]}
                        </Typography>
                        {
                            item.lastUpdated &&
                            <Typography variant="body2" sx={{ ...spacing(1) }}>
                                {item.lastUpdated}
                            </Typography>
                        }
                    </>
                } />
            </ListItemButton>
            <Divider />
        </>
    );
}
export default DirectoryInfo;