import React from 'react';
import { useTranslation } from 'react-i18next';
import { EditorField } from './Editorfield';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import styled from 'styled-components';
import { EditorContext } from '../../contexts/editorContext';
import { MultiLanguageString } from '../../types/multiLanguageString';
import { IEditorContentsResult } from '../../api/services/editor-contents';

interface IHeaderEditorProps {
    editorContentResponse: IEditorContentsResult;
    language: string;
    style: { [key: string]: string | number }
    maxLength?: number;
}

const Wrapper = styled.div`
  display: flex;
`;

const GridFixer = styled.div`
  grid-column: span 12;
`;

export const HeaderEditor: React.FC<IHeaderEditorProps> = ({ editorContentResponse, language, maxLength, style = {} }) => {
    const { t } = useTranslation();

    const { cubeQuery, setCubeQuery } = React.useContext(EditorContext);
    const editValue = cubeQuery?.chartHeaderEdit;
    const editHeader = (title: MultiLanguageString) => setCubeQuery({ ...cubeQuery, chartHeaderEdit: title })

    return (
        <GridFixer>
            <Wrapper>
                <InfoBubble info={t('infoText.titleEdition')} ariaLabel={t("editMetadata.header")} />
                <EditorField
                    label={t("editMetadata.header")}
                    style={style}
                    defaultValue={editorContentResponse.data?.headerText[language] ?? ""}
                    editValue={editValue ? editValue[language] : null}
                    onChange={newValue => {
                        const newEdit = { ...editValue, [language]: newValue };
                        editHeader(newEdit);
                    }}
                    maxLength={maxLength}
                />
            </Wrapper>
        </GridFixer>
    );
}

export default HeaderEditor;