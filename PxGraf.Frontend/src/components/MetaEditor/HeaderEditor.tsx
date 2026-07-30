import React from 'react';
import { debounce } from 'lodash';
import { useTranslation } from 'react-i18next';
import { EditorField } from './Editorfield';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import styled from 'styled-components';
import { QueryContext } from '../../contexts/queryContext';
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
    align-items: flex-start;
    gap: 4px;
    width: 100%;

    & > .MuiFormControl-root {
        flex: 1;
        min-width: 0;
    }
`;

const GridFixer = styled.div`
  grid-column: span 12;
`;

export const HeaderEditor: React.FC<IHeaderEditorProps> = ({ editorContentResponse, language, maxLength, style = {} }) => {
    const { t } = useTranslation();

    const { cubeQuery, setCubeQuery } = React.useContext(QueryContext);
    const editValue = cubeQuery?.chartHeaderEdit;
    const draftEditValue = React.useRef(editValue);
    const editHeader = React.useMemo(() => debounce((title: MultiLanguageString) => {
        setCubeQuery(currentCubeQuery => ({ ...currentCubeQuery, chartHeaderEdit: title }));
    }, 1000), [setCubeQuery]);

    React.useEffect(() => {
        draftEditValue.current = editValue;
    }, [editValue]);

    React.useEffect(() => () => editHeader.flush(), [editHeader]);

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
                        draftEditValue.current = { ...draftEditValue.current, [language]: newValue };
                        editHeader(draftEditValue.current);
                    }}
                    maxLength={maxLength}
                />
            </Wrapper>
        </GridFixer>
    );
}

export default HeaderEditor;