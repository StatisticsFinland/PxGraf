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
  max-height: 50vh;
  overflow-y: auto;
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
        const newDimensionEdit = {
            ...dimensionEdits,
            valueEdits: {
                ...dimensionEdits?.valueEdits,
                [valueCode]: {
                    ...dimensionEdits?.valueEdits?.[valueCode],
                    nameEdit: {
                        ...dimensionEdits?.valueEdits?.[valueCode]?.nameEdit,
                        [language]: newValue
                    }
                }
            }
        };

        setCubeQuery({
            ...cubeQuery,
            variableQueries: {
                ...cubeQuery?.variableQueries,
                [dimension.code]: newDimensionEdit
            }
        });
    };

    return (
        <EditorFieldWrapper>
            <Grid container spacing={3}>
                {dimension.values.map(value => {
                    return (
                        <Grid
                            key={value.code}
                            size={{ xs: 12, md: dimension.values.length > 5 ? 6 : 12 }}
                        >
                            <EditorField
                                label={t("editMetadata.valueName") + ": " + value.name[uiContentLanguage]}
                                defaultValue={value.name[language]}
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