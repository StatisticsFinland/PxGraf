import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack, Paper } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import EditorField from './Editorfield';
import styled from 'styled-components';
import { IDimensionValue } from 'types/cubeMeta';
import { IVariableValueEditions } from 'types/query';
import { getAdditionalProperty } from '../../utils/metadataUtils';

const EditorFieldWrapper = styled(Stack)`
  padding: 16px;
`;

interface IContentVariableValueEditorProps {
    variableValue: IDimensionValue;
    language: string;
    valueEdits: IVariableValueEditions;
    onChange: (newEdit: IVariableValueEditions) => void;
}

export const ContentVariableValueEditor: React.FC<IContentVariableValueEditorProps> = ({ variableValue, language, valueEdits, onChange }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const { t } = useTranslation();

    return (
        <Paper variant="outlined">
            <EditorFieldWrapper spacing={2}>
                <EditorField
                    label={t("editMetadata.valueName") + ": " + variableValue.Name[uiContentLanguage]}
                    defaultValue={variableValue.Name[language]}
                    editValue={valueEdits?.nameEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IVariableValueEditions = {
                            ...valueEdits,
                            nameEdit: {
                                ...valueEdits?.nameEdit,
                                [language]: newValue
                            }
                        };
                        onChange(newValueEdit);
                    }}
                />
                <EditorField
                    label={t("editMetadata.unit")}
                    defaultValue={variableValue?.Unit[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.unitEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IVariableValueEditions = {
                            ...valueEdits,
                            contentComponent: {
                                ...valueEdits?.contentComponent,
                                unitEdit: {
                                    ...valueEdits?.contentComponent?.unitEdit,
                                    [language]: newValue
                                }
                            }
                        };
                        onChange(newValueEdit);
                    }}
                />
                <EditorField
                    label={t("editMetadata.source")}
                    defaultValue={getAdditionalProperty("SOURCE", variableValue?.AdditionalProperties)[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.sourceEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IVariableValueEditions = {
                            ...valueEdits,
                            contentComponent: {
                                ...valueEdits?.contentComponent,
                                sourceEdit: {
                                    ...valueEdits?.contentComponent?.sourceEdit,
                                    [language]: newValue
                                }
                            }
                        };
                        onChange(newValueEdit);
                    }}
                />
            </EditorFieldWrapper>
        </Paper>
    );
}

export default ContentVariableValueEditor;