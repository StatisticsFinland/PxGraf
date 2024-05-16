import { Link } from "react-router-dom";
import { ListItemText, ListItemButton, Typography, Divider } from '@mui/material';
import { urls } from 'Router';
import React from "react";
import { spacing } from 'utils/componentHelpers';
import { MultiLanguageString } from "../../types/multiLanguageString";
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IDirectoryInfoProps {
    path: string;
    item: {
        id?: string;
        updated?: string;
        type?: 't' | 'l';
        text?: MultiLanguageString;
        languages?: string[];
    }
}

export const DirectoryInfo: React.FC<IDirectoryInfoProps> = ({ path, item }) => {
    const { language } = React.useContext(UiLanguageContext);
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    const currentPath = [...path, item.id];
    return (
        <>
            <ListItemButton id="mainContent" component={Link} to={urls.tableList(currentPath)}>
                <ListItemText primary={
                    <>
                        <Typography variant="body1" sx={{ ...spacing(1) }}>
                            {item.text[displayLanguage]}
                        </Typography>
                        {
                            item.updated &&
                            <Typography variant="body2" sx={{ ...spacing(1) }}>
                                {item.updated}
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