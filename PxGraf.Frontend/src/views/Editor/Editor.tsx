import React, { useEffect } from 'react';
import { useParams, useLocation } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import { Box, Stack, Divider, Container, CircularProgress, Alert } from '@mui/material';
import { EditorContext } from 'contexts/editorContext';
import { getDefaultQueries, getVisualizationOptionsForVisualizationType, resolveDimensions } from 'utils/editorHelpers';
import EditorFilterSection from './EditorFilterSection';
import EditorFooterSection from './EditorFooterSection';
import EditorPreviewSection from './EditorPreviewSection';
import EditorMetaSection from './EditorMetaSection';
import EditorDialogs from './EditorDialogs';
import styled from 'styled-components';
import { useCubeMetaQuery } from 'api/services/cube-meta';
import { useResolveDimensionFiltersQuery } from 'api/services/filter-dimension';
import { IFetchSavedQueryResponse, useSaveMutation } from 'api/services/queries';
import { extractCubeQuery, extractQuery } from 'utils/ApiHelpers';
import { useNavigationContext } from 'contexts/navigationContext';
import { useValidateTableMetadataQuery } from 'api/services/validate-table-metadata';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension } from '../../types/cubeMeta';
import { useEditorContentsQuery } from '../../api/services/editor-contents';
import { getValidatedSettings } from '../../utils/ChartSettingHelpers';

//Used to set the width of the dimension selection and preview margin in pixels
const dimensionSelectionWidth = 450;
//Max width in % for dimension selection and preview margin
const dimensionSelectionMaxWidthPercentage = 33;

const MetaPreviewSectionWrapper = styled(Box)`
    flex: 2 0;
    display: grid;
    gap: 0;
    grid-template-columns: 1fr;
    grid-template-rows: auto auto 1fr auto auto;
    grid-template-areas: 'parameters' 'parameters-preview-div' 'preview' 'preview-footer-div' 'footer';
    min-height: 100%;
    margin-left: ${dimensionSelectionWidth}px;

    @media (max-width: ${dimensionSelectionWidth / (dimensionSelectionMaxWidthPercentage / 100) }px) {
        margin-left: ${dimensionSelectionMaxWidthPercentage}%;
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
 * Contains the @see {@link EditorFilterSection} for defining values for each dimension, @see {@link EditorMetaSection} for editing the displayed meta data, @see {@link EditorPreviewSection} for previewing the chart and @see {@link EditorFooterSection} for saving a visualization.
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
        setQuery,
        setCubeQuery,
        setVisualizationSettingsUserInput,
        setSelectedVisualizationUserInput,
        defaultSelectables,
        setDefaultSelectables,
        loadedQueryId,
        setLoadedQueryId,
        loadedQueryIsDraft,
        setLoadedQueryIsDraft,
        setPublicationEnabled
    } = React.useContext(EditorContext);

    const { setTablePath } = useNavigationContext();

    useEffect(() => {
        if (result) {
            setQuery(extractQuery(result));
            setCubeQuery(extractCubeQuery(result));
            setVisualizationSettingsUserInput(result.settings);
            setSelectedVisualizationUserInput(result.settings.selectedVisualization);
            setDefaultSelectables(result.settings.defaultSelectableVariableCodes);
            setLoadedQueryId(result.id);
            setLoadedQueryIsDraft(result.draft);
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
    const isTableInvalid = tableValidityResponse.data && (!tableValidityResponse.data.allDimensionsContainValues || !tableValidityResponse.data.tableHasContentDimension || !tableValidityResponse.data.tableHasTimeDimension);
    const cubeMetaResponse = useCubeMetaQuery(path);
    const { language, languageTab, setLanguageTab, uiContentLanguage, setUiContentLanguage } = React.useContext(UiLanguageContext);
    const contentLanguages: string[] = cubeMetaResponse.data ? cubeMetaResponse.data.availableLanguages : [];

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

    const dimensions: IDimension[] = cubeMetaResponse.data?.dimensions ?? [];

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

    const editorContentsResponse = useEditorContentsQuery(path, modifiedQuery, cubeQuery);

    // Update publication enabled state when editor contents are loaded
    useEffect(() => {
        if (editorContentsResponse.data?.publicationEnabled !== undefined) {
            setPublicationEnabled(editorContentsResponse.data.publicationEnabled);
        }
    }, [editorContentsResponse.data?.publicationEnabled, setPublicationEnabled]);

    const resolvedDimensionCodesResponse = useResolveDimensionFiltersQuery(path, modifiedQuery);
    const resolvedDimensionCodes = React.useMemo(() => {
        if (resolvedDimensionCodesResponse.data != null) {
            return resolvedDimensionCodesResponse.data;
        }
        else if (dimensions != null) {
            const dimCodesNoVals = {}
            dimensions.forEach(v => {
                dimCodesNoVals[v.code] = [];
            })
            return dimCodesNoVals;
        }
        else {
            return {};
        }
    }, [resolvedDimensionCodesResponse, cubeMetaResponse.data]);
    const resolvedDimensions = React.useMemo(() => {
        if (cubeMetaResponse.data != null && dimensions != null) {
            return resolveDimensions(dimensions, resolvedDimensionCodes);
        }
        else {
            return null;
        }
    }, [cubeMetaResponse.data, resolvedDimensionCodes]);

    const selectedVisualization = React.useMemo(() => {
        if (editorContentsResponse.data?.visualizationOptions?.length > 0) {
            if (editorContentsResponse.data?.visualizationOptions?.some(options => options.type === selectedVisualizationUserInput)) {
                return selectedVisualizationUserInput;
            }
            else {
                return editorContentsResponse.data?.visualizationOptions[0].type;
            }
        }
        return null;
    }, [editorContentsResponse.data?.visualizationOptions, selectedVisualizationUserInput]);

    const visualizationSettings = React.useMemo(() => {
        const visualizationOptions = getVisualizationOptionsForVisualizationType(editorContentsResponse.data?.visualizationOptions, selectedVisualization);
        if (selectedVisualization != null && visualizationOptions?.sortingOptions != null && dimensions != null) {
            const sortingOptions = visualizationOptions?.allowManualPivot && visualizationSettingsUserInput?.pivotRequested ? visualizationOptions?.sortingOptions.pivoted : visualizationOptions?.sortingOptions.default;
            const result = getValidatedSettings(visualizationSettingsUserInput, selectedVisualization, sortingOptions, dimensions, modifiedQuery);
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
    }, [selectedVisualization, dimensions, visualizationSettingsUserInput, modifiedQuery, defaultSelectables, editorContentsResponse]);

    // If there is a loaded query (id) and it is a draft we use that id for saving, otherwise we save as a new query.
    const saveId = loadedQueryId && loadedQueryIsDraft ? loadedQueryId : '';
    const saveQueryMutation = useSaveMutation(path, modifiedQuery, cubeQuery, selectedVisualization, visualizationSettings, saveId);

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
    else if (isTableInvalid || tableValidityResponse.isError || cubeMetaResponse.isError || !cubeMetaResponse?.data?.dimensions || editorContentsResponse.isError) {
        const errorWithCubeMeta = cubeMetaResponse.isError || !cubeMetaResponse?.data?.dimensions;
        const errorConditionsAndMessages = [
            { condition: tableValidityResponse.isError || (errorWithCubeMeta && tableValidityResponse.data?.allDimensionsContainValues) || editorContentsResponse.isError, message: t("error.contentLoad") },
            { condition: tableValidityResponse.data && !tableValidityResponse.data.tableHasContentDimension, message: t("error.contentVariableMissing") },
            { condition: tableValidityResponse.data && !tableValidityResponse.data.tableHasTimeDimension, message: t("error.timeVariableMissing") },
            { condition: tableValidityResponse.data && !tableValidityResponse.data.allDimensionsContainValues, message: t("error.variablesMissingValues") },
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
                dimensions={dimensions}
                resolvedDimensionCodes={resolvedDimensionCodes}
                queries={modifiedQuery}
                width={dimensionSelectionWidth}
                maxWidthPercentage={dimensionSelectionMaxWidthPercentage}
            />
            <Divider orientation="vertical" />
            <MetaPreviewSectionWrapper>
                <EditorMetaSection
                    editorContentsResponse={editorContentsResponse}
                    resolvedDimensions={resolvedDimensions}
                    selectedVisualization={selectedVisualization}
                    dimensionQuery={modifiedQuery}
                    contentLanguages={contentLanguages}
                    visualizationSettings={visualizationSettings}
                />
                <PreviewDivider />
                <EditorPreviewSection
                    path={path}
                    query={modifiedQuery}
                    editorContents={editorContentsResponse}
                    visualizationSettings={visualizationSettings}
                    selectedVisualization={selectedVisualization}
                />
                <FooterDivider />
                <EditorFooterSection />
                <EditorDialogs saveQueryMutation={saveQueryMutation} />
            </MetaPreviewSectionWrapper>
        </Stack>
    );
}

export default Editor;
