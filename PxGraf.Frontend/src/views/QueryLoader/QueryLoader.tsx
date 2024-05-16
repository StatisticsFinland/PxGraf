import { fetchSavedQuery } from 'api/services/queries';
import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from "react-router-dom";
import { Container, CircularProgress, Alert } from '@mui/material';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';

const CubeMetaAlert = styled(Alert)`
  margin: 32px;
`;

enum PathProcessingState {
    Idle,
    Loading,
    Error
}

/**
 * Component used for loading a saved query using the query ID from the URL.
 */
export default function QueryLoader() {
    const { t } = useTranslation();
    const sqid = useParams()["*"];
    const [queryFetchError, setQueryFetchError] = useState("");
    const [pathProcessingState, setPathProcessingState] = useState(PathProcessingState.Idle);
    const navigate = useNavigate();

    useEffect(() => {
        setPathProcessingState(PathProcessingState.Loading);
        fetchQueryAndRedirect(sqid);
    }, [sqid]);

    React.useEffect(() => {
        document.title = `${t("pages.sqid")} | PxGraf`;
    }, []);

    const fetchQueryAndRedirect = async (queryId: string) => {
        try {
            const result = await fetchSavedQuery(queryId);
            const url = `/editor/${result.query.tableReference.hierarchy.join('/')}/${result.query.tableReference.name}/`
            navigate(url, { state: { result: result, queryId: queryId } });
        } catch (error) {
            setPathProcessingState(PathProcessingState.Error);
            setQueryFetchError(error.message);
        }
    }

    if (pathProcessingState === PathProcessingState.Loading) {
        return (
            <Container sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <CircularProgress role="loading" />
            </Container>
        );
    }
    else if (pathProcessingState === PathProcessingState.Error) {
        return (
            <Container sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'center' }}>
                <CubeMetaAlert severity="error">{t('error.queryLoad') + queryFetchError}</CubeMetaAlert>
            </Container>
        );
    }
}