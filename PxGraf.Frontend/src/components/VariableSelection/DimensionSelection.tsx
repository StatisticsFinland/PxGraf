import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

import { Stack, ListItemIcon, ListItemText, IconButton, MenuItem, Menu } from '@mui/material';

import MenuIcon from '@mui/icons-material/Menu';
import CheckIcon from '@mui/icons-material/Check';

import {
    getDefaultFilter, queryTypeLabels,
} from 'utils/dimensionSelectionHelpers';
import SelectabilitySwitch from './SelectabilitySwitch';
import ResultList from './ResultList';
import ManualPickDimensionSelection from './FilterComponents/ManualPickDimensionSelection';
import AllDimensionSelection from './FilterComponents/AllDimensionSelection';
import StartingFromDimensionSelection from './FilterComponents/StartingFromDimensionSelection';
import TopNDimensionSelection from './FilterComponents/TopNDimensionSelection';
import styled from 'styled-components';
import { IDimension } from 'types/cubeMeta';
import { FilterType, IDimensionQuery, Query } from 'types/query';
import DefaultSelectableDimensionSelection from './DefaultSelectableDimensionSelection';
import { EditorContext } from '../../contexts/editorContext';

interface IDimensionSelectionProps {
    dimension: IDimension
    resolvedDimensionValueCodes: string[]
    query: Query
}

const SelectorWrapper = styled(Stack)`
    width: 100%;
    align-items: flex-start;
`;

const ComponentWrapper = styled(Stack)`
    justify-content: space-between;
    align-items: flex-start;
`;

export const DimensionSelection: React.FC<IDimensionSelectionProps> = ({ dimension, resolvedDimensionValueCodes, query }) => {
    const [anchorEl, setAnchorEl] = useState(null);
    const { t } = useTranslation();
    const { setQuery } = React.useContext(EditorContext);
    const dimensionQuery = query[dimension.code];

    const onQueryChanged = (newQuery: IDimensionQuery) => {
        setQuery({
            ...query,
            [dimension.code]: newQuery
        });
    };

    const handleOpenMenuClick = (event) => {
        setAnchorEl(event.currentTarget);
    };

    const closeMenu = () => {
        setAnchorEl(null);
    };

    const handleMenuClick = (newFilterType: FilterType) => {
        closeMenu();
        onQueryChanged({
            ...dimensionQuery,
            valueFilter: getDefaultFilter(newFilterType)
        });
    };

    const handleFilterValueChanged = (newFilterValue: string | string[] | number) => {
        onQueryChanged({
            ...dimensionQuery,
            valueFilter: {
                type: dimensionQuery.valueFilter.type,
                query: newFilterValue
            }
        });
    }

    // MUI Switch onChange requires type (x: Y) => {} instead of (x: Y) => void
    const onChangeMUIWrapper = (newQuery: IDimensionQuery) => {
        onQueryChanged(newQuery);
        return {}
    }

    let filterComponent = null;
    let selectedValues = null;

    switch (dimensionQuery.valueFilter.type) {
        case FilterType.Item:
            if (dimensionQuery?.valueFilter?.query && dimension?.values) {
                const stringArray = dimensionQuery.valueFilter.query as string[];
                selectedValues = stringArray.map(code => dimension.values.find(o => o.code === code));
            }
            filterComponent =
                <ManualPickDimensionSelection
                    options={dimension.values}
                    selectedValues={selectedValues}
                    onQueryChanged={handleFilterValueChanged}
                />
            break;
        case FilterType.All:
            filterComponent =
                <AllDimensionSelection />
            break;
        case FilterType.From:
            filterComponent =
                <StartingFromDimensionSelection
                    options={dimension.values}
                    startingCode={dimensionQuery.valueFilter.query as string}
                    onQueryChanged={handleFilterValueChanged}
                />
            break;
        case FilterType.Top:
            filterComponent =
                <TopNDimensionSelection
                    numberOfItems={dimensionQuery.valueFilter.query as number}
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
                dimensionQuery.valueFilter.type !== FilterType.Item ? (
                    <ResultList dimensionValues={dimension.values} resolvedDimensionValueCodes={resolvedDimensionValueCodes} />
                ) : null
            }

            <SelectabilitySwitch onChange={value => onChangeMUIWrapper({ ...dimensionQuery, selectable: value })} selected={dimensionQuery.selectable} />
            {
                dimensionQuery.selectable && <DefaultSelectableDimensionSelection dimensionCode={dimension.code} resolvedDimensionValueCodes={resolvedDimensionValueCodes} options={dimension.values} />
            }

            <Menu open={anchorEl != null} anchorEl={anchorEl} onClose={closeMenu}>
                {
                    (Object.values(FilterType) as Array<FilterType>).map(queryType => {
                        const selected = dimensionQuery.valueFilter.type === queryType;
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
export default DimensionSelection;