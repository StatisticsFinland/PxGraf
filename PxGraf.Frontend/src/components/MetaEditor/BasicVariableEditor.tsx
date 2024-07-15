import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EditorField } from './Editorfield';
import styled from 'styled-components';
import { IVariable } from 'types/cubeMeta';
import { IVariableEditions } from 'types/query';

const EditorFieldWrapper = styled(Stack)`
  padding: 0px;
`;

interface IBasicVariableEditorProps {
    variable: IVariable;
    language: string;
    variableEdits: IVariableEditions;
    onChange: (newEdit: IVariableEditions) => void;
}

export const BasicVariableEditor: React.FC<IBasicVariableEditorProps> = ({ variable, language, variableEdits, onChange }) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <EditorFieldWrapper spacing={2}>
            {variable.values.map(variableValue => {
                return (
                    <EditorField
                        label={t("editMetadata.valueName") + ": " + variableValue.name[uiContentLanguage]}
                        key={variableValue.code}
                        defaultValue={variableValue.name[language]}
                        editValue={variableEdits?.valueEdits[variableValue.code]?.nameEdit?.[language]}
                        onChange={newValue => {
                            const newVariableEdit = {
                                ...variableEdits,
                                valueEdits: {
                                    ...variableEdits?.valueEdits,
                                    [variableValue.code]: {
                                        ...variableEdits?.valueEdits[variableValue.code],
                                        nameEdit: {
                                            ...variableEdits?.valueEdits[variableValue.code]?.nameEdit,
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