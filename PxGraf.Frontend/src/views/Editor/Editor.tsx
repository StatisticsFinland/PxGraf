import React, { useEffect } from 'react';
import { useParams, useLocation } from "react-router-dom";
import { useTranslation } from 'react-i18next';

import { Box, Stack, Divider, Container, CircularProgress, Alert } from '@mui/material';

import { EditorContext } from 'contexts/editorContext';
import { getDefaultQueries, resolveVariables } from 'utils/editorHelpers';
import { getValidatedSettings } from 'utils/ChartSettingHelpers';
import EditorFilterSection from './EditorFilterSection';
import EditorFooterSection from './EditorFooterSection';
import EditorPreviewSection from './EditorPreviewSection';
import EditorMetaSection from './EditorMetaSection';
import EditorDialogs from './EditorDialogs';
import styled from 'styled-components';
import { useCubeMetaQuery } from 'api/services/cube-meta';
import { useDefaultHeaderQuery } from 'api/services/default-header';
import { useResolveVariableFiltersQuery } from 'api/services/filter-dimension';
import { useVisualizationOptionsQuery } from 'api/services/visualization-rules';
import { IFetchSavedQueryResponse, useSaveMutation } from 'api/services/queries';
import { VisualizationType } from 'types/visualizationType';
import { useQueryInfoQuery } from 'api/services/query-info';
import { extractCubeQuery, extractQuery } from 'utils/ApiHelpers';
import { useNavigationContext } from 'contexts/navigationContext';
import { useValidateTableMetadataQuery, IValidateTableMetaDataResult } from 'api/services/validate-table-metadata';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension } from '../../types/cubeMeta';

//Used to set the width of the variable selection and preview margin in pixels
const variableSelectionWidth = 450;
//Max width in % for variable selection and preview margin
const variableSelectionMaxWidthPercentage = 33;

const MetaPreviewSectionWrapper = styled(Box)`
    flex: 2 0;
    display: grid;
    gap: 0;
    grid-template-columns: 1fr;
    grid-template-rows: auto auto 1fr auto auto;
    grid-template-areas: 'parameters' 'parameters-preview-div' 'preview' 'preview-footer-div' 'footer';
    min-height: 100%;
    margin-left: ${variableSelectionWidth}px;

    @media (max-width: ${variableSelectionWidth / (variableSelectionMaxWidthPercentage / 100) }px) {
        margin-left: ${variableSelectionMaxWidthPercentage}%;
    }
`;

const PreviewDivider = styled(Divider)`
  grid-area: 'parameters-preview-div';
`;

const FooterDivider = styled(Divider)`
  grid-area: 'preview-footer-div';
`;

const CubeMetaAlert = styled(Alert)`
  margin: 32px;
`;

/**
 * Main component for the editor view.
 * Contains the @see {@link EditorFilterSection} for defining values for each variable, @see {@link EditorMetaSection} for editing the displayed meta data, @see {@link EditorPreviewSection} for previewing the chart and @see {@link EditorFooterSection} for saving a visualization.
 */
export const Editor = () => {

    const location = useLocation();
    const { result }: { result: IFetchSavedQueryResponse } = location?.state ? location.state : { result: null };

    React.useEffect(() => {
        document.title = `${t("pages.editor")} | PxGraf`;
    }, []);

    // statemanagement
    const {
        cubeQuery,
        query,
        selectedVisualizationUserInput,
        visualizationSettingsUserInput,
        defaultSelectables,
        setQuery,
        setCubeQuery,
        setVisualizationSettingsUserInput,
        setSelectedVisualizationUserInput,
        setDefaultSelectables
    } = React.useContext(EditorContext);

    const { setTablePath } = useNavigationContext();

    useEffect(() => {
        if (result) {
            setQuery(extractQuery(result));
            setCubeQuery(extractCubeQuery(result));
            setVisualizationSettingsUserInput(result.settings);
            setSelectedVisualizationUserInput(result.settings.selectedVisualization);
            setDefaultSelectables(result.settings.defaultSelectableVariableCodes);
        }
    }, [result]);

    // hooks and support functions
    const { t } = useTranslation();
    const params = useParams();
    const pathStr = params["*"];
    const path = React.useMemo(
        () => pathStr.split("/").filter(p => p.length > 0),
        [pathStr]
    );

    useEffect(() => {
        if (path.length) {
            setTablePath(path);
        }
    }, [path]);

    // queries and other functionality
    const tableValidityResponse = useValidateTableMetadataQuery(path);
    const isTableInvalid = (response: IValidateTableMetaDataResult) => {
        if (response.data) {
            return !response.data.allVariablesContainValues || !response.data.tableHasContentVariable || !response.data.tableHasTimeVariable
        }
        else {
            return !response.isLoading;
        }
    }

    const tableIsInvalid = isTableInvalid(tableValidityResponse);
    const cubeMetaResponse = useCubeMetaQuery(path);

    const { language, languageTab, setLanguageTab, uiContentLanguage, setUiContentLanguage } = React.useContext(UiLanguageContext);

    const contentLanguages: string[] = cubeMetaResponse.data ? cubeMetaResponse.data.AvailableLanguages : [];

    useEffect(() => {
        if (!contentLanguages || contentLanguages.length == 0) {
            return;
        }
        if (contentLanguages.includes(language)) {
            setUiContentLanguage(language);
        }
        else if (!uiContentLanguage) {
            const contentLanguage = contentLanguages.includes(languageTab) ? languageTab : contentLanguages[0];
            setLanguageTab(contentLanguages[0]);
            setUiContentLanguage(contentLanguage)
        }
    }, [language, contentLanguages]);

    const dimensions: IDimension[] = cubeMetaResponse.data?.Dimensions ?? [];

    const modifiedQuery = React.useMemo(() => {
        if (query != null) {
            return query;
        }
        else if (dimensions != null) {
            return getDefaultQueries(dimensions);
        }
        else {
            return null;
        }
    }, [query, cubeMetaResponse.data]);

    const defaultHeaderResponse = useDefaultHeaderQuery(path, modifiedQuery);
    const queryInfoResponse = useQueryInfoQuery(path, modifiedQuery, cubeQuery);

    const resolvedVariableCodesResponse = useResolveVariableFiltersQuery(path, modifiedQuery);
    const resolvedVariableCodes = React.useMemo(() => {
        if (resolvedVariableCodesResponse.data != null) {
            return resolvedVariableCodesResponse.data;
        }
        else if (dimensions != null) {
            const varCodesNoVals = {}
            dimensions.forEach(v => {
                varCodesNoVals[v.Code] = [];
            })
            return varCodesNoVals;
        }
        else {
            return {};
        }
    }, [resolvedVariableCodesResponse, cubeMetaResponse.data]);
    const resolvedVariables = React.useMemo(() => {
        if (cubeMetaResponse.data != null && dimensions != null) {
            return resolveVariables(dimensions, resolvedVariableCodes);
        }
        else {
            return null;
        }
    }, [cubeMetaResponse.data, resolvedVariableCodes]);
    const selectedVisualization = React.useMemo(() => {
        if (queryInfoResponse.data?.validVisualizations.length > 0) {
            if (queryInfoResponse.data?.validVisualizations.includes(selectedVisualizationUserInput)) {
                return selectedVisualizationUserInput;
            }
            else {
                return queryInfoResponse.data?.validVisualizations[0] as VisualizationType;
            }
        }
        return null;
    }, [queryInfoResponse.data?.validVisualizations, selectedVisualizationUserInput]);
    const visualizationRulesResponse = useVisualizationOptionsQuery(
        path,
        modifiedQuery,
        selectedVisualization,
        // We should NOT use visualizationSettingsUserInput but since getDefaultSettings always return pivotRequested as false (or null) we can getaway with this cheat.
        visualizationSettingsUserInput?.pivotRequested ?? false,
    );
    const visualizationSettings = React.useMemo(() => {
        if (selectedVisualization != null && visualizationRulesResponse.data?.sortingOptions != null && resolvedVariables != null) {
            const result = getValidatedSettings(visualizationSettingsUserInput, selectedVisualization, visualizationRulesResponse.data.sortingOptions, resolvedVariables, modifiedQuery);
            if (defaultSelectables && Object.keys(defaultSelectables).length > 0) {
                result.defaultSelectableVariableCodes = defaultSelectables;
            } else {
                result.defaultSelectableVariableCodes = null;
            }
            return result;
        }
        else {
            return null;
        }
    }, [visualizationSettingsUserInput, selectedVisualization, visualizationRulesResponse.data?.sortingOptions, resolvedVariables, modifiedQuery, defaultSelectables]);
    const saveQueryMutation = useSaveMutation(path, modifiedQuery, cubeQuery, selectedVisualization, visualizationSettings);

    const errorContainer = (errorMessage: string) => {
        return (
            <Container sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'center' }}>
                <CubeMetaAlert id="mainContent" severity="error">{errorMessage}</CubeMetaAlert>
            </Container>
        );
    }

    // return spinner or alert depending if results are loading or returns error 
    if (cubeMetaResponse.isLoading || tableValidityResponse.isLoading) {
        return (
            <Container sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <CircularProgress id="mainContent" />
            </Container>
        );
    }
    else if (tableIsInvalid || cubeMetaResponse.isError || !cubeMetaResponse?.data || !dimensions) {
        const errorWithCubeMeta = cubeMetaResponse.isError || !cubeMetaResponse?.data || !dimensions;
        const errorConditionsAndMessages = [
            { condition: tableValidityResponse.isError || (errorWithCubeMeta && tableValidityResponse.data?.allVariablesContainValues), message: t("error.contentLoad") },
            { condition: !tableValidityResponse.data?.tableHasContentVariable, message: t("error.contentVariableMissing") },
            { condition: !tableValidityResponse.data?.tableHasTimeVariable, message: t("error.timeVariableMissing") },
            { condition: !tableValidityResponse.data?.allVariablesContainValues, message: t("error.variablesMissingValues") }
        ];
        const errorMessages = errorConditionsAndMessages
            .filter(item => item.condition)
            .map(item => item.message);

        const errorMessage = errorMessages.join(" ");
        return errorContainer(errorMessage);
    }

    // return the actual component
    return (
        <Stack direction="row">
            <EditorFilterSection
                variables={dimensions}
                resolvedVariableCodes={resolvedVariableCodes}
                queries={modifiedQuery}
                width={variableSelectionWidth}
                maxWidthPercentage={variableSelectionMaxWidthPercentage}
            />
            <Divider orientation="vertical" />
            <MetaPreviewSectionWrapper>
                <EditorMetaSection
                    defaultHeaderResponse={defaultHeaderResponse}
                    visualizationRulesResponse={visualizationRulesResponse}
                    queryInfo={queryInfoResponse.data}
                    resolvedVariables={resolvedVariables}
                    selectedVisualization={selectedVisualization}
                    settings={visualizationSettings}
                    variableQuery={modifiedQuery}
                    contentLanguages={contentLanguages}
                />
                <PreviewDivider />
                <EditorPreviewSection
                    path={path}
                    query={modifiedQuery}
                    queryInfo={queryInfoResponse.data}
                    selectedVisualization={selectedVisualization}
                    visualizationSettings={visualizationSettings}
                />
                <FooterDivider />
                <EditorFooterSection />
                <EditorDialogs saveQueryMutation={saveQueryMutation} />
            </MetaPreviewSectionWrapper>
        </Stack>
    );
}

export default Editor;
