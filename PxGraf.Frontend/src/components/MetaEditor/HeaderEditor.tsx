import React from 'react';
import { useTranslation } from 'react-i18next';

import { Alert, Skeleton } from '@mui/material';
import { EditorField } from './Editorfield';
import { MultiLanguageString } from 'types/multiLanguageString';
import { IHeaderResult } from 'api/services/default-header';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import styled from 'styled-components';

interface IHeaderEditorProps {
    defaultHeaderResponse: IHeaderResult;
    language: string;
    editValue: MultiLanguageString;
    onChange: (newEdit: MultiLanguageString) => void;
    style: { [key: string]: string | number }
    maxLength?: number;
}

const Wrapper = styled.div`
  display: flex;
`;

const GridFixer = styled.div`
  grid-column: span 12;
`;

export const HeaderEditor: React.FC<IHeaderEditorProps> = ({ defaultHeaderResponse, language, editValue, onChange, maxLength, style = {} }) => {
    const { t } = useTranslation();

    if (defaultHeaderResponse.isError) {
        return (
            <GridFixer>
                <Alert style={style} severity="error">{t("error.contentLoad")}</Alert>
            </GridFixer>
        );
    }
    else if (defaultHeaderResponse.isLoading) {
        return (
            <GridFixer>
                <Skeleton style={style} variant="rectangular" height={55} />
            </GridFixer>
        );
    }
    else {
        return (
            <GridFixer>
                <Wrapper>
                    <InfoBubble info={t('infoText.titleEdition')} ariaLabel={t("editMetadata.header")} />
                    <EditorField
                        label={t("editMetadata.header")}
                        style={style}
                        defaultValue={defaultHeaderResponse.data[language]}
                        editValue={editValue ? editValue[language] : null}
                        onChange={newValue => {
                            const newEdit = { ...editValue, [language]: newValue };
                            onChange(newEdit);
                        }}
                        maxLength={maxLength}
                    />
                </Wrapper>
            </GridFixer>
        );
    }
}

export default HeaderEditor;