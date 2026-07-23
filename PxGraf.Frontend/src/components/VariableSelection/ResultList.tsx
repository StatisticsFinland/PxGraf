import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack, List, ListItem, ListItemText, Typography, Skeleton } from '@mui/material';
import styled from 'styled-components';
import { IDimensionValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IResultListProps {
    dimensionValues: IDimensionValue[]
    resolvedDimensionValueCodes: string[] | null | undefined
}

const DenseList = styled(List)`
    width: 100%;
    border-style: solid;
    border-width: 1px;
    border-radius: 4px;
    position: relative;
    overflow: auto;
    max-height: 200px;
`;

const NoPaddingListItem = styled(ListItem)`
    padding-top: 0;
    padding-bottom: 0;
`;

const StyledEm = styled.em`
    color: var(--text-muted);
`;

const StyledSkeleton = styled(Skeleton)`
    width: 50%;
`;

const DenseListWrapper = styled(Stack)`
    width: 100%;
`;

export const ResultList: React.FC<IResultListProps> = ({ dimensionValues, resolvedDimensionValueCodes }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const resultsTitleId = React.useId();

    let listContent;
    if (resolvedDimensionValueCodes == null) {
        listContent =
            <NoPaddingListItem>
                <StyledSkeleton variant="text" />
            </NoPaddingListItem>
    }
    else if (resolvedDimensionValueCodes.length === 0) {
        listContent =
            <NoPaddingListItem>
                <StyledEm>{t("general.noResults")}</StyledEm>
            </NoPaddingListItem>
    }
    else {
        const resultTexts = resolvedDimensionValueCodes.map(valueCode => {
            const value = dimensionValues.find(value => value.code === valueCode);
            return value?.name[uiContentLanguage] ?? valueCode;
        });

        listContent = resultTexts.map((value) =>
            <NoPaddingListItem key={value}>
                <ListItemText primary={value} />
            </NoPaddingListItem>
        );
    }

    let statusText = t('general.loading');
    if (resolvedDimensionValueCodes != null) {
        statusText = resolvedDimensionValueCodes.length === 0
            ? t('general.noResults')
            : t('general.resultCount', { count: resolvedDimensionValueCodes.length });
    }

    return (
        <DenseListWrapper>
            <Typography id={resultsTitleId}>{t("general.results")}:</Typography>
            <Typography role="status" sx={{ position: 'absolute', width: 1, height: 1, padding: 0, margin: -1, overflow: 'hidden', clip: 'rect(0 0 0 0)', whiteSpace: 'nowrap', border: 0 }}>
                {statusText}
            </Typography>
            <DenseList dense
                aria-labelledby={resultsTitleId}
                tabIndex={0}
                sx={{
                    bgcolor: 'background.paper',
                    borderColor: 'divider',
                    '&:focus-visible': { outline: '2px solid', outlineColor: 'primary.main', outlineOffset: 2 },
                    '& ul': { padding: 0 },
                }}
            >
                {listContent}
            </DenseList>
        </DenseListWrapper>
    );
}

export default ResultList;