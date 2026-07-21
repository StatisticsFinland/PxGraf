import { List, ListItemButton, ListItemText } from '@mui/material';

import React from 'react';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension } from 'types/cubeMeta';
import styled from 'styled-components';

const ListWrapper = styled.div`
    flex: 1;
    min-width: 200px;
    border: 1px solid rgba(0, 0, 0, 0.23);
    border-radius: 4px;
    overflow: hidden;
`;

const ListHeader = styled.div`
    padding: 4px 16px;
    font-size: 0.875rem;
    font-weight: 500;
    color: rgba(0, 0, 0, 0.6);
    background-color: #fff;
    border-bottom: 1px solid rgba(0, 0, 0, 0.12);
    line-height: 2;
`;

const ScrollableList = styled(List)`
    height: 84px;
    overflow-y: auto;
    padding: 0;

    .MuiListItemButton-root {
        min-height: unset;
        padding: 4px 12px;
    }

    .MuiListItemButton-root.Mui-selected {
        background-color: transparent;
        border: 1px solid #1976d2;
        border-radius: 2px;
        color: #1976d2;
    }

    .MuiListItemButton-root.Mui-selected:hover {
        background-color: rgba(25, 118, 210, 0.08);
    }

    .MuiListItemText-root {
        margin: 0;
    }
`;

interface IDimensionListProps {
    title: string,
    dimensions: IDimension[],
    selectedDimensionCode: string,
    selectedChangedHandler: (newCode: string) => void,
}

const dimensionNameAndValues = (dimension: IDimension, uiContentLanguage: string): string => {
    const name: string = dimension.name[uiContentLanguage] ?? dimension.code;
    const values: number = dimension.values.length;
    return name + " " + values;
};

export const DimensionList: React.FC<IDimensionListProps> = ({ title, dimensions, selectedDimensionCode, selectedChangedHandler }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <ListWrapper>
            <ListHeader>{title}</ListHeader>
            <ScrollableList dense={true}>
                {dimensions.map(v => (
                    <ListItemButton
                        key={"var-" + v.code}
                        onClick={() => selectedChangedHandler(v.code)}
                        selected={selectedDimensionCode === v.code}
                    >
                        <ListItemText primary={dimensionNameAndValues(v, uiContentLanguage)} />
                    </ListItemButton>
                ))}
            </ScrollableList>
        </ListWrapper>
    );
}

export default DimensionList;