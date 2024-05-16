import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

import { Stack, ListItemIcon, ListItemText, IconButton, MenuItem, Menu } from '@mui/material';

import MenuIcon from '@mui/icons-material/Menu';
import CheckIcon from '@mui/icons-material/Check';

import {
    getDefaultFilter, queryTypeLabels,
} from 'utils/variableSelectionHelpers';
import SelectabilitySwitch from './SelectabilitySwitch';
import ResultList from './ResultList';
import ManualPickVariableSelection from './FilterComponents/ManualPickVariableSelection';
import AllVariableSelection from './FilterComponents/AllVariableSelection';
import StartingFromVariableSelection from './FilterComponents/StartingFromVariableSelection';
import TopNVariableSelection from './FilterComponents/TopNVariableSelection';
import styled from 'styled-components';
import { IVariable } from 'types/cubeMeta';
import { FilterType, IVariableQuery } from 'types/query';
import DefaultSelectableVariableSelection from './DefaultSelectableVariableSelection';

interface IVariableSelectionProps {
    variable: IVariable
    resolvedVariableValueCodes: string[]
    query: IVariableQuery
    onQueryChanged: (newVarQuery: IVariableQuery) => void
}

const SelectorWrapper = styled(Stack)`
    width: 100%;
    align-items: flex-start;
`;

const ComponentWrapper = styled(Stack)`
    justify-content: space-between;
    align-items: flex-start;
`;

export const VariableSelection: React.FC<IVariableSelectionProps> = ({ variable, resolvedVariableValueCodes, query, onQueryChanged }) => {
    const [anchorEl, setAnchorEl] = useState(null);
    const { t } = useTranslation();

    const handleOpenMenuClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const closeMenu = () => {
        setAnchorEl(null);
    };

    const handleMenuClick = (newFilterType: FilterType) => {
        closeMenu();
        onQueryChanged({
            ...query,
            valueFilter: getDefaultFilter(newFilterType)
        });
    };

    const handleFilterValueChanged = (newFilterValue: string | string[] | number) => {
        onQueryChanged({
            ...query,
            valueFilter: {
                type: query.valueFilter.type,
                query: newFilterValue
            }
        });
    }

    // MUI Switch onChange requires type (x: Y) => {} instead of (x: Y) => void
    const onChangeMUIWrapper = (newQuery: IVariableQuery) => {
        onQueryChanged(newQuery);
        return {}
    }

    let filterComponent = null;

    let selectedValues = null;


    switch (query.valueFilter.type) {
        case FilterType.Item:
            if (query?.valueFilter?.query && variable?.values) {
                const stringArray = query.valueFilter.query as string[];
                selectedValues = stringArray.map(code => variable.values.find(o => o.code === code));
            }
            filterComponent =
                <ManualPickVariableSelection
                options={variable.values}
                selectedValues={selectedValues}
                onQueryChanged={handleFilterValueChanged}
                />
            break;
        case FilterType.All:
            filterComponent =
                <AllVariableSelection />
            break;
        case FilterType.From:
            filterComponent =
                <StartingFromVariableSelection
                options={variable.values}
                startingCode={query.valueFilter.query as string}
                onQueryChanged={handleFilterValueChanged}
                />
            break;
        case FilterType.Top:
            filterComponent =
                <TopNVariableSelection
                    numberOfItems={query.valueFilter.query as number}
                    onNumberChanged={handleFilterValueChanged}
                />
            break;
    }

    return (
        <ComponentWrapper direction="column" spacing={2}>
            <SelectorWrapper direction="row">
                <IconButton aria-label={anchorEl ? t('variableSelect.closeMenu') : t('variableSelect.openMenu')} component="span" style={{ paddingLeft: "0px", marginRight: "12px" }} onClick={handleOpenMenuClick}>
                    <MenuIcon />
                </IconButton>
                {filterComponent}
            </SelectorWrapper>

            {
                query.valueFilter.type !== FilterType.Item ? (
                    <ResultList variableValues={variable.values} resolvedVariableValueCodes={resolvedVariableValueCodes} />
                ) : null
            }

            <SelectabilitySwitch onChange={value => onChangeMUIWrapper({ ...query, selectable: value })} selected={query.selectable} />
            {
                query.selectable && <DefaultSelectableVariableSelection variableCode={variable.code} resolvedVariableValueCodes={resolvedVariableValueCodes} options={variable.values} />
            }

            <Menu open={anchorEl != null} anchorEl={anchorEl} onClose={closeMenu}>
                {
                    (Object.values(FilterType) as Array<FilterType>).map(queryType => {
                        const selected = query.valueFilter.type === queryType;
                        return (
                            <MenuItem key={queryType} selected={selected} onClick={() => handleMenuClick(queryType)}>
                                {selected ?
                                    <><ListItemIcon><CheckIcon /></ListItemIcon>{t(queryTypeLabels[queryType])}</> :
                                    <ListItemText inset>{t(queryTypeLabels[queryType])}</ListItemText>
                                }
                            </MenuItem>
                        );
                    })
                }
            </Menu>
        </ComponentWrapper>
    );
}
export default VariableSelection;