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

interface VariableListProps {
    title: string,
    variables: IDimension[],
    selectedVariableCode: string,
    selectedChangedHandler: (newCode: string) => void,
}

const variableNameAndValues = (variable: IDimension, uiContentLanguage: string): string => {
    const name: string = variable.name[uiContentLanguage] ?? variable.code;
    const values: number = variable.values.length;
    return name + " " + values;
};

export const VariableList: React.FC<VariableListProps> = ({ title, variables, selectedVariableCode, selectedChangedHandler }) => {
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    return (
        <StyledList
            subheader={<ListSubheader>{title}</ListSubheader>}
            dense={true}
        >
            {variables.map(v => {
                return (
                    <ListItemButton
                        key={"var-" + v.code}
                        onClick={() => selectedChangedHandler(v.code)}
                        selected={selectedVariableCode === v.code}>
                        <ListItemText primary={variableNameAndValues(v, uiContentLanguage)} />
                    </ListItemButton>
                );
            })}
        </StyledList>
    );
}

export default VariableList;