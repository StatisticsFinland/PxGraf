import { Link } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import { ListItemText, ListItemButton, Typography, Divider } from '@mui/material';
import React from 'react';
import { urls } from 'Router';
import { spacing } from 'utils/componentHelpers';
import UiLanguageContext from 'contexts/uiLanguageContext';
import { IDatabaseTable } from 'types/tableListItems';

interface ITableInfoProps {
    path: string;
    item: IDatabaseTable;
}

export const TableInfo: React.FC<ITableInfoProps> = ({ path, item }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const currentPath = [path, item.fileName];
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