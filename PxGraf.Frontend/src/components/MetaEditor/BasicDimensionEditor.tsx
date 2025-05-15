import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EditorField } from './Editorfield';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { EditorContext } from '../../contexts/editorContext';

const EditorFieldWrapper = styled(Stack)`
  padding: 0px;
`;

interface IBasicDimensionEditor {
    dimension: IDimension;
    language: string;
}

export const BasicDimensionEditor: React.FC<IBasicDimensionEditor> = ({ dimension, language }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const { cubeQuery, setCubeQuery } = React.useContext(EditorContext);
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
        <EditorFieldWrapper spacing={2}>
            {dimension.values.map(value => {
                return (
                    <EditorField
                        label={t("editMetadata.valueName") + ": " + value.name[uiContentLanguage]}
                        key={value.code}
                        defaultValue={value.name[language]}
                        editValue={dimensionEdits?.valueEdits[value.code]?.nameEdit?.[language]}
                        onChange={newValue => handleChange(newValue, value.code)}
                    />
                );
            })}
        </EditorFieldWrapper>
    );
}

export default BasicDimensionEditor;