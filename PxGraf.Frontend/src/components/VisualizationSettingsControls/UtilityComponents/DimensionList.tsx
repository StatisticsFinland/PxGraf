import { List, ListItemButton, ListItemText, ListSubheader } from '@mui/material';

import React from 'react';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension } from 'types/cubeMeta';
import styled from 'styled-components';

const StyledList = styled(List)`
    height: 100%;
    width: 45%;
    min-width: 175px;
    max-width: 300px;
    border: 1px solid;
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
        <StyledList
            subheader={<ListSubheader>{title}</ListSubheader>}
            dense={true}
        >
            {dimensions.map(v => {
                return (
                    <ListItemButton
                        key={"var-" + v.code}
                        onClick={() => selectedChangedHandler(v.code)}
                        selected={selectedDimensionCode === v.code}>
                        <ListItemText primary={dimensionNameAndValues(v, uiContentLanguage)} />
                    </ListItemButton>
                );
            })}
        </StyledList>
    );
}

export default DimensionList;