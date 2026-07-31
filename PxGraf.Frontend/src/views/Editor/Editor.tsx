import React, { useEffect } from 'react';
import { useParams, useLocation } from "react-router-dom";
import { useTranslation } from 'react-i18next';
import i18n from 'i18next';
import { Box, Divider, Container, CircularProgress, Alert, Snackbar } from '@mui/material';
import { QueryContext } from 'contexts/queryContext';
import { VisualizationContext } from 'contexts/visualizationContext';
import { SaveContext } from 'contexts/saveContext';
import { getDefaultQueries, getVisualizationOptionsForVisualizationType, resolveDimensions, enrichDimensionsWithVirtualValues, getVirtualValueDefinitionsSignature } from 'utils/editorHelpers';
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
import { VirtualValueOperator } from 'types/query';
import { useEditorContentsQuery } from '../../api/services/editor-contents';
import { getValidatedSettings } from '../../utils/ChartSettingHelpers';
import { EPreviewSize } from 'types/previewSize';

//Used to set the width of the dimension selection and preview margin in pixels
const dimensionSelectionWidth = 450;
//Max width in % for dimension selection and preview margin
const dimensionSelectionMaxWidthPercentage = 33;

const EditorLayout = styled(Box)`
    display: flex;
    flex-direction: row;
    height: 100%;
    overflow: hidden;
`;

const MetaPreviewSectionWrapper = styled(Box)`
    flex: 1;
    min-width: 0;
    display: grid;
    gap: 0;
    grid-template-columns: 1fr;
    grid-template-rows: auto auto 1fr auto auto;
    grid-template-areas: 'parameters' 'parameters-preview-div' 'preview' 'preview-footer-div' 'footer';
    overflow: hidden;
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
    const [dismissedRecoveryLocationKey, setDismissedRecoveryLocationKey] = React.useState<string | null>(null);
    const recoveryAlertOpen = Boolean(result?.recoveredWithChanges && dismissedRecoveryLocationKey !== location.key);

    // hooks and support functions
    const { t } = useTranslation();

    React.useEffect(() => {
        document.title = `${t("pages.editor")} | PxGraf`;
    }, [t]);

    // statemanagement
    const {
        cubeQuery, setCubeQuery,
        query, setQuery,
    } = React.useContext(QueryContext);
    const {
        selectedVisualizationUserInput, setSelectedVisualizationUserInput,
        visualizationSettingsUserInput, setVisualizationSettingsUserInput,
        defaultSelectables, setDefaultSelectables,
    } = React.useContext(VisualizationContext);
    const {
        loadedQueryId, setLoadedQueryId,
        loadedQueryIsDraft, setLoadedQueryIsDraft,
        setPublicationWebhookEnabled,
    } = React.useContext(SaveContext);

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
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: context setters are stable, only re-run when result changes
    }, [result]);

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
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: setTablePath is a stable context setter
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
        // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: only react to language/contentLanguages changes, other deps would cause loops
    }, [language, contentLanguages]);

    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: dimensions reference changes with cubeMetaResponse.data which triggers dependent memos
    const dimensions: IDimension[] = cubeMetaResponse.data?.dimensions ?? [];

    const modifiedQuery = React.useMemo(() => {
        if (query != null) {
            return query;
        }
        else if (cubeMetaResponse.data == null) {
            return null;
        }
        else {
            return getDefaultQueries(dimensions);
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: dimensions is derived from cubeMetaResponse.data which is already a dep
    }, [query, cubeMetaResponse.data]);

    const editorContentsResponse = useEditorContentsQuery(path, modifiedQuery, cubeQuery);

    // Update publication enabled state when editor contents are loaded
    useEffect(() => {
        if (editorContentsResponse.data?.publicationWebhookEnabled !== undefined) {
            setPublicationWebhookEnabled(editorContentsResponse.data.publicationWebhookEnabled);
        }
    }, [editorContentsResponse.data?.publicationWebhookEnabled, setPublicationWebhookEnabled]);

    const resolvedDimensionCodesResponse = useResolveDimensionFiltersQuery(path, modifiedQuery);
    const resolvedDimensionCodes = React.useMemo(() => {
        if (resolvedDimensionCodesResponse.data != null) {
            return resolvedDimensionCodesResponse.data;
        }
        else if (cubeMetaResponse.data == null) {
            return {};
        }
        else {
            const dimCodesNoVals = {}
            dimensions.forEach(v => {
                dimCodesNoVals[v.code] = [];
            })
            return dimCodesNoVals;
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: dimensions is derived from cubeMetaResponse.data which is already a dep
    }, [resolvedDimensionCodesResponse, cubeMetaResponse.data]);
    const resolvedDimensions = React.useMemo(() => {
        if (cubeMetaResponse.data != null && dimensions != null) {
            return resolveDimensions(dimensions, resolvedDimensionCodes);
        }
        else {
            return null;
        }
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: dimensions is derived from cubeMetaResponse.data which is already a dep
    }, [cubeMetaResponse.data, resolvedDimensionCodes]);

    const virtualValueDefinitionsSignature = getVirtualValueDefinitionsSignature(dimensions, modifiedQuery);

    const enrichedDimensions = React.useMemo(() => {
        const availableLanguages = cubeMetaResponse.data?.availableLanguages ?? [];
        const translateForLang = (lang: string, operator: VirtualValueOperator): string => {
            const key = `computedValues.operator${operator.charAt(0).toUpperCase()}${operator.slice(1)}`;
            return i18n.t(key, { lng: lang });
        };
        return enrichDimensionsWithVirtualValues(dimensions, modifiedQuery, availableLanguages, translateForLang, cubeQuery);
    // eslint-disable-next-line react-hooks/exhaustive-deps -- intentional: only virtual value definitions from modifiedQuery affect enrichment
    }, [cubeMetaResponse.data, virtualValueDefinitionsSignature, cubeQuery]);

    const enrichedResolvedDimensions = React.useMemo(() => {
        if (!resolvedDimensions) return null;
        const availableLanguages = cubeMetaResponse.data?.availableLanguages ?? [];
        const translateForLang = (lang: string, operator: VirtualValueOperator): string => {
            const key = `computedValues.operator${operator.charAt(0).toUpperCase()}${operator.slice(1)}`;
            return i18n.t(key, { lng: lang });
        };
        return enrichDimensionsWithVirtualValues(resolvedDimensions, modifiedQuery, availableLanguages, translateForLang, null, true);
    }, [resolvedDimensions, modifiedQuery, cubeMetaResponse.data]);

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

    const [previewSize, setPreviewSize] = React.useState<EPreviewSize>(EPreviewSize.Desktop);
    const [metadataDialogOpen, setMetadataDialogOpen] = React.useState(false);

    const errorContainer = (errorMessage: string) => {
        return (
            <Container sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'center' }}>
                <CubeMetaAlert severity="error">{errorMessage}</CubeMetaAlert>
            </Container>
        );
    }

    // return spinner or alert depending if results are loading or returns error 
    if (cubeMetaResponse.isLoading || tableValidityResponse.isLoading) {
        return (
            <Container sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <CircularProgress />
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
        <>
        <Snackbar
            open={recoveryAlertOpen}
            anchorOrigin={{ vertical: 'top', horizontal: 'center' }}
        >
            <Alert
                severity="warning"
                variant="outlined"
                onClose={() => setDismissedRecoveryLocationKey(location.key)}
                sx={{
                    backgroundColor: 'background.paper',
                    borderWidth: 2,
                    boxShadow: 6,
                    '& .MuiAlert-message': {
                        fontSize: '1rem',
                    },
                    '& .MuiAlert-icon': {
                        alignSelf: 'center',
                        fontSize: 28,
                        paddingBlock: 0,
                    },
                    '& .MuiAlert-action': {
                        alignSelf: 'center',
                        paddingBlock: 0,
                    },
                    '& .MuiAlert-action .MuiSvgIcon-root': {
                        fontSize: 24,
                    },
                }}
            >
                {t('warning.savedQueryPartiallyRecovered')}
            </Alert>
        </Snackbar>
        <EditorLayout>
            <EditorFilterSection
                dimensions={enrichedDimensions}
                resolvedDimensionCodes={resolvedDimensionCodes}
                queries={modifiedQuery}
                width={dimensionSelectionWidth}
                maxWidthPercentage={dimensionSelectionMaxWidthPercentage}
                onEditMetadata={() => setMetadataDialogOpen(true)}
            />
            <Divider orientation="vertical" />
            <MetaPreviewSectionWrapper>
                <EditorMetaSection
                    editorContentsResponse={editorContentsResponse}
                    resolvedDimensions={enrichedResolvedDimensions}
                    selectedVisualization={selectedVisualization}
                    dimensionQuery={modifiedQuery}
                    contentLanguages={contentLanguages}
                    visualizationSettings={visualizationSettings}
                    previewSize={previewSize}
                    onPreviewSizeChange={setPreviewSize}
                />
                <PreviewDivider />
                <EditorPreviewSection
                    path={path}
                    query={modifiedQuery}
                    editorContents={editorContentsResponse}
                    visualizationSettings={visualizationSettings}
                    selectedVisualization={selectedVisualization}
                    previewSize={previewSize}
                />
                <FooterDivider />
                <EditorFooterSection
                    size={editorContentsResponse.data?.size}
                    maximumSize={editorContentsResponse.data?.maximumSupportedSize}
                    warningLimit={editorContentsResponse.data?.sizeWarningLimit}
                />
                <EditorDialogs
                    saveQueryMutation={saveQueryMutation}
                    metadataDialogOpen={metadataDialogOpen}
                    metadataDimensions={enrichedResolvedDimensions}
                    contentLanguages={contentLanguages}
                    metadataLanguage={languageTab}
                    cubeQuery={cubeQuery}
                    onMetadataApply={variableQueries => setCubeQuery(currentCubeQuery => ({
                        ...currentCubeQuery,
                        variableQueries,
                    }))}
                    onMetadataClose={() => setMetadataDialogOpen(false)}
                />
            </MetaPreviewSectionWrapper>
        </EditorLayout>
        </>
    );
}

export default Editor;
