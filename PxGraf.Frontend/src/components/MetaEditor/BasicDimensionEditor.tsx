import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EditorField } from './Editorfield';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { IDimensionEditions } from 'types/query';

const EditorFieldWrapper = styled(Stack)`
  padding: 0px;
`;

interface IBasicDimensionEditor {
    dimension: IDimension;
    language: string;
    dimensionEdits: IDimensionEditions;
    onChange: (newEdit: IDimensionEditions) => void;
}

export const BasicDimensionEditor: React.FC<IBasicDimensionEditor> = ({ dimension, language, dimensionEdits, onChange }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <EditorFieldWrapper spacing={2}>
            {dimension.values.map(value => {
                return (
                    <EditorField
                        label={t("editMetadata.valueName") + ": " + value.name[uiContentLanguage]}
                        key={value.code}
                        defaultValue={value.name[language]}
                        editValue={dimensionEdits?.valueEdits[value.code]?.nameEdit?.[language]}
                        onChange={newValue => {
                            const newDimensionEdit = {
                                ...dimensionEdits,
                                valueEdits: {
                                    ...dimensionEdits?.valueEdits,
                                    [value.code]: {
                                        ...dimensionEdits?.valueEdits[value.code],
                                        nameEdit: {
                                            ...dimensionEdits?.valueEdits[value.code]?.nameEdit,
                                            [language]: newValue
                                        }
                                    }
                                }
                            };
                            onChange(newDimensionEdit);
                        }}
                    />
                );
            })}
        </EditorFieldWrapper>
    );
}

export default BasicDimensionEditor;