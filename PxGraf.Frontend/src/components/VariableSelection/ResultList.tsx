import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack, List, ListItem, ListItemText, Typography, Skeleton } from '@mui/material';
import styled from 'styled-components';
import { IDimensionValue } from 'types/cubeMeta';
import { UiLanguageContext } from 'contexts/uiLanguageContext';

interface IResultListProps {
    variableValues: IDimensionValue[]
    resolvedVariableValueCodes: string[]
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
    color: #ccc;
`;

const StyledSkeleton = styled(Skeleton)`
    width: 50%;
`;

const DenseListWrapper = styled(Stack)`
    width: 100%;
`;

export const ResultList: React.FC<IResultListProps> = ({ variableValues, resolvedVariableValueCodes }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    let listContent;
    if (resolvedVariableValueCodes == null) {
        listContent =
            <NoPaddingListItem>
                <StyledSkeleton variant="text" />
            </NoPaddingListItem>
    }
    else if (resolvedVariableValueCodes.length === 0) {
        listContent =
            <NoPaddingListItem>
                <StyledEm>{t("general.noResults")}</StyledEm>
            </NoPaddingListItem>
    }
    else {
        const resultTexts = resolvedVariableValueCodes.map(valueCode => {
            const value = variableValues.find(value => value.Code === valueCode);
            return value?.Name[uiContentLanguage] ?? valueCode;
        });

        listContent = resultTexts.map((value) =>
            <NoPaddingListItem key={value}>
                <ListItemText primary={value} />
            </NoPaddingListItem>
        );
    }

    return (
        <DenseListWrapper>
            <Typography>{t("general.results")}:</Typography>
            <DenseList dense
                sx={{
                    bgcolor: 'background.paper',
                    borderColor: 'divider',
                    '& ul': { padding: 0 },
                }}
            >
                {listContent}
            </DenseList>
        </DenseListWrapper>
    );
}

export default ResultList;