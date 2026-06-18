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
    const [pathProcessingState, setPathProcessingState] = useState(sqid ? PathProcessingState.Loading : PathProcessingState.Idle);
    const navigate = useNavigate();

    const fetchQueryAndRedirect = React.useCallback(async (queryId: string) => {
        try {
            const result = await fetchSavedQuery(queryId);
            const url = `/editor/${result.query.tableReference.hierarchy.join('/')}/${result.query.tableReference.name}/`
            navigate(url, { state: { result: result, queryId: queryId } });
        } catch (error: unknown) {
            setPathProcessingState(PathProcessingState.Error);
            setQueryFetchError(error instanceof Error ? error.message : String(error));
        }
    }, [navigate]);

    useEffect(() => {
        if (!sqid) return;
        // eslint-disable-next-line react-hooks/set-state-in-effect -- setState calls in fetchQueryAndRedirect happen asynchronously after await, not synchronously in the effect body
        fetchQueryAndRedirect(sqid);
    }, [sqid, fetchQueryAndRedirect]);

    React.useEffect(() => {
        document.title = `${t("pages.sqid")} | PxGraf`;
    }, [t]);

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

    return null;
}
