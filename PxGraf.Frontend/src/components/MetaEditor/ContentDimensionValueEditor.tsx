import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack, Paper } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import EditorField from './Editorfield';
import styled from 'styled-components';
import { IContentDimensionValue } from 'types/cubeMeta';
import { IDimensionValueEditions } from 'types/query';
import { getAdditionalPropertyValue } from '../../utils/metadataUtils';
import { sourceKey } from '../../utils/keywordConstants';

const EditorFieldWrapper = styled(Stack)`
  padding: 16px;
`;

interface IContentDimensionValueEditorProps {
    dimensionValue: IContentDimensionValue;
    language: string;
    valueEdits: IDimensionValueEditions;
    onChange: (newEdit: IDimensionValueEditions) => void;
}

export const ContentDimensionValueEditor: React.FC<IContentDimensionValueEditorProps> = ({ dimensionValue, language, valueEdits, onChange }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const { t } = useTranslation();

    return (
        <Paper variant="outlined">
            <EditorFieldWrapper spacing={2}>
                <EditorField
                    label={t("editMetadata.valueName") + ": " + dimensionValue.name[uiContentLanguage]}
                    defaultValue={dimensionValue.name[language]}
                    editValue={valueEdits?.nameEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IDimensionValueEditions = {
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
                    defaultValue={dimensionValue?.unit[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.unitEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IDimensionValueEditions = {
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
                    defaultValue={getAdditionalPropertyValue(sourceKey, dimensionValue?.additionalProperties)[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.sourceEdit?.[language]}
                    onChange={newValue => {
                        const newValueEdit: IDimensionValueEditions = {
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

export default ContentDimensionValueEditor;