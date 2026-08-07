import { List, ListItemButton, ListItemText } from '@mui/material';

import React from 'react';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension } from 'types/cubeMeta';
import { styled } from '@mui/material/styles';

const ListWrapper = styled('div')(({ theme }) => ({
    flex: 1,
    minWidth: 200,
    border: `1px solid ${theme.palette.divider}`,
    borderRadius: theme.shape.borderRadius,
    overflow: 'hidden',
}));

const ListHeader = styled('div')(({ theme }) => ({
    padding: '4px 16px',
    ...theme.typography.body2,
    fontWeight: theme.typography.fontWeightMedium,
    color: theme.palette.text.secondary,
    backgroundColor: theme.palette.background.paper,
    borderBottom: `1px solid ${theme.palette.divider}`,
    lineHeight: 2,
}));

const ScrollableList = styled(List)(({ theme }) => ({
    height: 84,
    overflowY: 'auto',
    padding: 0,
    '& .MuiListItemButton-root': {
        minHeight: 'unset',
        padding: '4px 12px',
    },
    '& .MuiListItemButton-root.Mui-selected': {
        backgroundColor: 'transparent',
        border: `1px solid ${theme.palette.primary.main}`,
        borderRadius: theme.shape.borderRadius,
        color: theme.palette.primary.main,
    },
    '& .MuiListItemButton-root.Mui-selected:hover': {
        backgroundColor: theme.palette.primary.light,
    },
    '& .MuiListItemText-root': { margin: 0 },
}));

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