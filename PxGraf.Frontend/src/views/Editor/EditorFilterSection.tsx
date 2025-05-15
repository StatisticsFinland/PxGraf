import DimensionSelectionList from 'components/VariableSelection/DimensionSelectionList';
import React from 'react';
import { Box } from '@mui/material';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';

interface EditorFilterSectionProps {
    dimensions: IDimension[],
    resolvedDimensionCodes: { [key: string]: string[] }
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
 * Component for the filter section in the editor. Contains @see {@link DimensionSelectionList} for each dimension for filtering values and defining selectable dimensionss.
 * @param {IDimension[]} dimensions Dimensions available for the table.
 * @param {{[key:string]: string[]}} resolvedDimensionCodes Codes for the resolved dimension values.
 * @param {Query} queries Object that contains dimension queries
 * @param {number} width Width of the dimension filter section
 * @param {number} maxWidthPercentage Maximum width of the dimension filter section on the whole window defined in percentages
 */
export const EditorFilterSection: React.FC<EditorFilterSectionProps> = ({ dimensions, resolvedDimensionCodes, queries, width, maxWidthPercentage }) => {
    
    return (
        <SelectorWrapper width={width} $maxWidthPercentage={maxWidthPercentage}>
            <DimensionSelectionList
                dimensions={dimensions}
                resolvedDimensionCodes={resolvedDimensionCodes}
                query={queries}
            />
        </SelectorWrapper>
    )
}

export default EditorFilterSection;