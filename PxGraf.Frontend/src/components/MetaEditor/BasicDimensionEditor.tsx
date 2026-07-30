import React from 'react';
import { useTranslation } from 'react-i18next';
import { Grid } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EditorField } from './Editorfield';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { QueryContext } from '../../contexts/queryContext';

const EditorFieldWrapper = styled.div`
  padding-top: 0.5rem;
  padding-right: 1rem;
`;

interface IBasicDimensionEditor {
    dimension: IDimension;
    language: string;
}

export const BasicDimensionEditor: React.FC<IBasicDimensionEditor> = ({ dimension, language }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const { cubeQuery, setCubeQuery } = React.useContext(QueryContext);
    const dimensionEdits = cubeQuery?.variableQueries[dimension.code];

    const handleChange = (newValue: string, valueCode: string) => {
        setCubeQuery(currentCubeQuery => {
            const currentDimensionEdits = currentCubeQuery.variableQueries[dimension.code];
            return {
                ...currentCubeQuery,
                variableQueries: {
                    ...currentCubeQuery.variableQueries,
                    [dimension.code]: {
                        ...currentDimensionEdits,
                        valueEdits: {
                            ...currentDimensionEdits?.valueEdits,
                            [valueCode]: {
                                ...currentDimensionEdits?.valueEdits?.[valueCode],
                                nameEdit: {
                                    ...currentDimensionEdits?.valueEdits?.[valueCode]?.nameEdit,
                                    [language]: newValue
                                }
                            }
                        }
                    }
                }
            };
        });
    };

    return (
        <EditorFieldWrapper>
            <Grid container spacing={3}>
                {dimension.values.map(value => {
                    return (
                        <Grid
                            key={value.code}
                            size={{ xs: 12, md: dimension.values.length >= 4 ? 6 : 12 }}
                        >
                            <EditorField
                                label={t("editMetadata.valueName") + ": " + (value.name[uiContentLanguage] ?? dimensionEdits?.valueEdits?.[value.code]?.nameEdit?.[uiContentLanguage] ?? value.code)}
                                defaultValue={value.name[language] ?? ''}
                                editValue={dimensionEdits?.valueEdits?.[value.code]?.nameEdit?.[language]}
                                onChange={newValue => handleChange(newValue, value.code)}
                                style={{ width: '100%' }}
                            />
                        </Grid>
                    );
                })}
            </Grid>
        </EditorFieldWrapper>
    );
}

export default BasicDimensionEditor;