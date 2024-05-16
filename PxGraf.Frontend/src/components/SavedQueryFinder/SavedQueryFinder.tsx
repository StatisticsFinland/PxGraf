import React from 'react';
import { Button, Dialog, DialogTitle, DialogContent, DialogActions, Alert, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import SearchIcon from '@mui/icons-material/Search';
import { fetchSavedQuery } from 'api/services/queries';
import { useNavigate } from "react-router-dom";
import styled from 'styled-components';


const AlertWrapper = styled.div`
    margin-top: 15px;
`;

interface ISavedQueryFinderProps {
    oldQueryId?: string;
}

export const SavedQueryFinder: React.FC<ISavedQueryFinderProps> = ({ oldQueryId }) => {
    const [open, setOpen] = React.useState(false);
    const [queryId, setQueryId] = React.useState(oldQueryId || '');
    const [isLoading, setIsLoading] = React.useState(false);
    const [errorString, setErrorString] = React.useState('');
    const { t } = useTranslation();
    const navigate = useNavigate();

    const fetchOldQueryAndRedirect = async (queryId: string) => {
        setIsLoading(true);
        try {
            const result = await fetchSavedQuery(queryId);
            if (result) {
                const url = `/editor/${result.query.tableReference.hierarchy.join('/')}/${result.query.tableReference.name}/`
                navigate(url, { state: { result: result, queryId: queryId } });
                setIsLoading(false);
                setOpen(false);
            } else {
                setErrorString(t('savedQuery.errorMsg'));
                setIsLoading(false);
            }
        } catch (error) {
            console.log(error);
            setErrorString(t('savedQuery.errorMsg'));
            setIsLoading(false);
        }

    }

    return (
        <>
            <Button variant={'outlined'} onClick={() => setOpen(true)}>{t('savedQuery.dialogButtonTxt')}</Button>
            <Dialog
                open={open}
                onClose={() => setOpen(false)}
                fullWidth
                maxWidth="xs"
            >
                <DialogTitle id='query-dialog-title'>{t('savedQuery.dialogTitleTxt')}</DialogTitle>
                <DialogContent dividers={true}>
                    <TextField variant='outlined' fullWidth label={t('savedQuery.inputLabel')} value={queryId} disabled={isLoading} onChange={(event) => { setQueryId(event.target.value); setErrorString(''); }} />
                    {errorString.length > 0 && <AlertWrapper aria-live='polite'><Alert severity={"error"}>{errorString}</Alert></AlertWrapper>}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpen(false)}>{t('savedQuery.closeButton')}</Button>
                    <Button variant={'contained'} disabled={isLoading} startIcon={<SearchIcon />} onClick={() => fetchOldQueryAndRedirect(queryId)}>{isLoading ? t('savedQuery.searchingTxt') : t('savedQuery.searchTxt')}</Button>
                </DialogActions>
            </Dialog>
        </>
    )
};

export default SavedQueryFinder;