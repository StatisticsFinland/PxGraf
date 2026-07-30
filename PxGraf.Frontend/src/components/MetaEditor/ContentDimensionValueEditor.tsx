import React from 'react';
import { useTranslation } from 'react-i18next';
import { Stack, Paper } from '@mui/material';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import EditorField from './Editorfield';
import styled from 'styled-components';
import { IContentDimensionValue } from 'types/cubeMeta';
import { IDimensionValueEditions } from 'types/query';
import { MultiLanguageString } from 'types/multiLanguageString';
import { getAdditionalPropertyValue } from '../../utils/metadataUtils';
import { sourceKey } from '../../utils/keywordConstants';

const EditorFieldWrapper = styled(Stack)`
  padding: 16px;
`;

interface IContentDimensionValueEditorProps {
    dimensionValue: IContentDimensionValue;
    language: string;
    valueEdits: IDimensionValueEditions;
    onChange: React.Dispatch<React.SetStateAction<IDimensionValueEditions>>;
}

export const ContentDimensionValueEditor: React.FC<IContentDimensionValueEditorProps> = ({ dimensionValue, language, valueEdits, onChange }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);
    const { t } = useTranslation();

    return (
        <Paper variant="outlined">
            <EditorFieldWrapper spacing={2}>
                <EditorField
                    label={t("editMetadata.valueName") + ": " + (dimensionValue.name[uiContentLanguage] ?? valueEdits?.nameEdit?.[uiContentLanguage] ?? dimensionValue.code)}
                    defaultValue={dimensionValue.name[language]}
                    editValue={valueEdits?.nameEdit?.[language]}
                    onChange={newValue => {
                        onChange(currentValueEdits => ({
                            ...currentValueEdits,
                            nameEdit: {
                                ...currentValueEdits?.nameEdit,
                                [language]: newValue
                            }
                        }));
                    }}
                />
                <EditorField
                    label={t("editMetadata.unit")}
                    defaultValue={dimensionValue?.unit?.[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.unitEdit?.[language]}
                    onChange={newValue => {
                        onChange(currentValueEdits => ({
                            ...currentValueEdits,
                            contentComponent: {
                                ...currentValueEdits?.contentComponent,
                                unitEdit: {
                                    ...currentValueEdits?.contentComponent?.unitEdit,
                                    [language]: newValue
                                }
                            }
                        }));
                    }}
                />
                <EditorField
                    label={t("editMetadata.source")}
                    defaultValue={(getAdditionalPropertyValue(sourceKey, dimensionValue?.additionalProperties) as MultiLanguageString)?.[language] ?? ''}
                    editValue={valueEdits?.contentComponent?.sourceEdit?.[language]}
                    onChange={newValue => {
                        onChange(currentValueEdits => ({
                            ...currentValueEdits,
                            contentComponent: {
                                ...currentValueEdits?.contentComponent,
                                sourceEdit: {
                                    ...currentValueEdits?.contentComponent?.sourceEdit,
                                    [language]: newValue
                                }
                            }
                        }));
                    }}
                />
            </EditorFieldWrapper>
        </Paper>
    );
}

export default ContentDimensionValueEditor;