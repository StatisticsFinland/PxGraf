import React from 'react';
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    IconButton,
    Tab,
    Tabs,
} from '@mui/material';
import CheckIcon from '@mui/icons-material/Check';
import CloseIcon from '@mui/icons-material/Close';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';
import { QueryContext } from 'contexts/queryContext';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import TabPanel from 'components/TabPanel/TabPanel';
import { IDimension } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import DimensionEditor from './DimensionEditor';

interface IMetadataDialogProps {
    open: boolean;
    dimensions: IDimension[];
    contentLanguages: string[];
    initialLanguage: string;
    initialCubeQuery: ICubeQuery;
    onApply: (variableQueries: ICubeQuery['variableQueries']) => void;
    onClose: () => void;
}

const StyledDialogTitle = styled(DialogTitle)`
    display: flex;
    align-items: center;
    justify-content: space-between;
`;

const TabsWrapper = styled.div`
    border-bottom: 1px solid var(--border-light);
    flex: 0 0 auto;
`;

const StyledDialogContent = styled(DialogContent)`
    display: flex;
    flex-direction: column;
    min-height: 0;
    overflow: hidden;
`;

const ValuesScroller = styled.div`
    flex: 1;
    min-height: 0;
    overflow-y: auto;
`;

export const MetadataDialog: React.FC<IMetadataDialogProps> = ({
    open,
    dimensions,
    contentLanguages,
    initialLanguage,
    initialCubeQuery,
    onApply,
    onClose,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const draftCubeQueryRef = React.useRef(initialCubeQuery);
    const wasOpenRef = React.useRef(false);
    const dimensionTabsId = React.useId();
    const languageTabsId = React.useId();
    const [renderedDraftCubeQuery, setRenderedDraftCubeQuery] = React.useState(initialCubeQuery);
    const [language, setLanguage] = React.useState(initialLanguage);
    const [dimensionTab, setDimensionTab] = React.useState(0);

    const updateDraftCubeQuery = React.useCallback((update: React.SetStateAction<ICubeQuery>) => {
        draftCubeQueryRef.current = typeof update === 'function'
            ? update(draftCubeQueryRef.current)
            : update;
    }, []);

    const draftContextValue = React.useMemo(() => ({
        cubeQuery: renderedDraftCubeQuery,
        setCubeQuery: updateDraftCubeQuery,
        query: null,
        setQuery: () => { /* metadata draft does not edit dimension filters */ },
    }), [renderedDraftCubeQuery, updateDraftCubeQuery]);

    React.useEffect(() => {
        if (open && !wasOpenRef.current) {
            draftCubeQueryRef.current = initialCubeQuery;
            setRenderedDraftCubeQuery(initialCubeQuery);
            setLanguage(initialLanguage);
            setDimensionTab(0);
        }
        wasOpenRef.current = open;
    }, [open, initialCubeQuery, initialLanguage]);

    const applyAndClose = () => {
        onApply(draftCubeQueryRef.current.variableQueries);
        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={onClose}
            fullWidth
            maxWidth="lg"
            scroll="paper"
            slotProps={{
                paper: {
                    sx: {
                        height: {
                            xs: 'calc(100dvh - 32px)',
                            sm: 'min(760px, calc(100dvh - 64px))',
                        },
                    },
                },
            }}
            aria-labelledby="metadata-dialog-title"
        >
            <StyledDialogTitle id="metadata-dialog-title">
                {t('editMetadata.dialogTitle')}
                <IconButton onClick={onClose} aria-label={t('editMetadata.close')}>
                    <CloseIcon />
                </IconButton>
            </StyledDialogTitle>
            <StyledDialogContent dividers>
                <QueryContext.Provider value={draftContextValue}>
                    <TabsWrapper>
                        <Tabs
                            value={dimensionTab}
                            onChange={(_, nextTab) => {
                                setRenderedDraftCubeQuery(draftCubeQueryRef.current);
                                setDimensionTab(nextTab);
                            }}
                            variant="scrollable"
                            scrollButtons="auto"
                            aria-label={t('editMetadata.dimensions')}
                        >
                            {dimensions.map((dimension, index) => (
                                <Tab
                                    key={dimension.code}
                                    label={dimension.name[uiContentLanguage] ?? dimension.code}
                                    id={`${dimensionTabsId}-${index}`}
                                    aria-controls={`${dimensionTabsId}-panel-${index}`}
                                />
                            ))}
                        </Tabs>
                    </TabsWrapper>
                    <TabsWrapper>
                        <Tabs
                            value={language}
                            onChange={(_, nextLanguage) => {
                                setRenderedDraftCubeQuery(draftCubeQueryRef.current);
                                setLanguage(nextLanguage);
                            }}
                            aria-label={t('editor.contentLanguage')}
                        >
                            {contentLanguages.map(contentLanguage => (
                                <Tab
                                    key={contentLanguage}
                                    value={contentLanguage}
                                    label={contentLanguage}
                                    aria-label={`${t('editor.contentLanguage')}: ${t('lang.local.' + contentLanguage)}`}
                                    id={`${languageTabsId}-${contentLanguage}`}
                                    aria-controls={`${dimensionTabsId}-panel-${dimensionTab}`}
                                />
                            ))}
                        </Tabs>
                    </TabsWrapper>
                    <ValuesScroller>
                        {dimensions.map((dimension, index) => (
                            <TabPanel key={dimension.code} selectedValue={dimensionTab} value={index} idPrefix={dimensionTabsId}>
                                <DimensionEditor dimension={dimension} language={language} />
                            </TabPanel>
                        ))}
                    </ValuesScroller>
                </QueryContext.Provider>
            </StyledDialogContent>
            <DialogActions>
                <Button onClick={onClose}>{t('editMetadata.cancel')}</Button>
                <Button onClick={applyAndClose} variant="contained" startIcon={<CheckIcon />}>
                    {t('editMetadata.apply')}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default MetadataDialog;
