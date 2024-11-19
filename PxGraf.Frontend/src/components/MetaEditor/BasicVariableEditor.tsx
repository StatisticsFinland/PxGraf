import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EditorField } from './Editorfield';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';

const EditorFieldWrapper = styled(Stack)`
  padding: 0px;
`;

interface IBasicVariableEditorProps {
    variable: IDimension;
    language: string;
    variableEdits: IVariableEditions;
    onChange: (newEdit: IVariableEditions) => void;
}

export const BasicVariableEditor: React.FC<IBasicVariableEditorProps> = ({ variable, language, variableEdits, onChange }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <EditorFieldWrapper spacing={2}>
            {variable.values.map(value => {
                return (
                    <EditorField
                        label={t("editMetadata.valueName") + ": " + value.name[uiContentLanguage]}
                        key={value.code}
                        defaultValue={value.name[language]}
                        editValue={variableEdits?.valueEdits[value.code]?.nameEdit?.[language]}
                        onChange={newValue => {
                            const newVariableEdit = {
                                ...variableEdits,
                                valueEdits: {
                                    ...variableEdits?.valueEdits,
                                    [value.code]: {
                                        ...variableEdits?.valueEdits[value.code],
                                        nameEdit: {
                                            ...variableEdits?.valueEdits[value.code]?.nameEdit,
                                            [language]: newValue
                                        }
                                    }
                                }
                            };
                            onChange(newVariableEdit);
                        }}
                    />
                );
            })}
        </EditorFieldWrapper>
    );
}

export default BasicVariableEditor;