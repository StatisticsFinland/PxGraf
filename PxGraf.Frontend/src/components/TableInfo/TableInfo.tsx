import { Link } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import { ListItemText, ListItemButton, Typography, Divider } from '@mui/material';

import { urls } from 'Router';
import React from "react";
import { spacing } from 'utils/componentHelpers';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { MultiLanguageString } from "../../types/multiLanguageString";

interface ITableInfoProps {
    path: string;
    item: {
        code?: string;
        lastUpdated?: string;
        name?: MultiLanguageString;
        languages?: string[];
    };
}

export const TableInfo: React.FC<ITableInfoProps> = ({ path, item }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const currentPath = [path, item.code];
    const displayLanguage = item.languages.includes(language) ? language : item.languages[0];

    return (
        <>
            <ListItemButton id="mainContent" component={Link} to={urls.editor(currentPath)}>
                <ListItemText primary={
                    <>
                        <Typography variant="body1" sx={{ ...spacing(1) }}>
                            {item.name[displayLanguage]}
                        </Typography>
                        <Typography variant="body2" sx={{ ...spacing(1) }}>
                            {t("tableSelect.updated") + ": " + new Date(item.lastUpdated).toLocaleString(language)}
                        </Typography>
                    </>
                } />
            </ListItemButton>
            <Divider />
        </>
    );
}

export default TableInfo;