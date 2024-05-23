import VariableSelectionList from 'components/VariableSelection/VariableSelectionList';
import React from 'react';
import { Box } from '@mui/material';
import { EditorContext } from 'contexts/editorContext';
import styled from 'styled-components';
import { IVariable } from 'types/cubeMeta';
import { Query } from 'types/query';

interface EditorFilterSectionProps {
    variables: IVariable[],
    resolvedVariableCodes: { [key: string]: string[] }
    queries: Query
    width?: number
    maxWidthPercentage?: number
}

const SelectorWrapper = styled(Box)<{width: number, $maxWidthPercentage: number}>`
  max-width: ${props => props.$maxWidthPercentage}%;
  flex: 0 1 ${props => props.width}px;
  position: fixed;
  top: 0;
  width: ${props => props.width}px;
  height: calc(100vh - 100px);
  margin-top: 98px;
  overflow: auto;
  padding-bottom: 50px;
  background-color: white;
  z-index: 5;
  border-top: thin solid rgba(0, 0, 0, 0.12);

  @media (max-width: 980px) {
    margin-top: 144px;
  }
`;

/**
 * Component for the filter section in the editor. Contains @see {@link VariableSelectionList} for each variable for filtering values and defining selectable variables.
 * @param {IVariable[]} variables Variables available for the table.
 * @param {{[key:string]: string[]}} resolvedVariableCodes Codes for the resolved variable values.
 * @param {Query} queries Object that contains variable queries
 * @param {number} width Width of the variable filter section
 * @param {number} maxWidthPercentage Maximum width of the variable filter section on the whole window defined in percentages
 */
export const EditorFilterSection: React.FC<EditorFilterSectionProps> = ({ variables, resolvedVariableCodes, queries, width, maxWidthPercentage }) => {
    
    const { setQuery } = React.useContext(EditorContext);

    return (
        <SelectorWrapper width={width} $maxWidthPercentage={maxWidthPercentage}>
            <VariableSelectionList
                variables={variables}
                resolvedVariableCodes={resolvedVariableCodes}
                query={queries}
                onQueryChanged={setQuery}
            />
        </SelectorWrapper>
    )
}

export default EditorFilterSection;