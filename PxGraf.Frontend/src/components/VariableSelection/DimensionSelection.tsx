import React from 'react';
import { useTranslation } from 'react-i18next';

import { Stack, FormControl, InputLabel, Select, MenuItem, SelectChangeEvent, IconButton, Popover, Tooltip, Typography, Badge } from '@mui/material';
import CheckIcon from '@mui/icons-material/Check';
import TouchAppIcon from '@mui/icons-material/TouchApp';

import {
    getDefaultFilter, queryTypeLabels,
} from 'utils/dimensionSelectionHelpers';
import SelectabilitySwitch from './SelectabilitySwitch';
import ResultList from './ResultList';
import ManualPickDimensionSelection from './FilterComponents/ManualPickDimensionSelection';
import StartingFromDimensionSelection from './FilterComponents/StartingFromDimensionSelection';
import TopNDimensionSelection from './FilterComponents/TopNDimensionSelection';
import RegexDimensionSelection from './FilterComponents/RegexDimensionSelection';
import styled from 'styled-components';
import { IDimensionValue } from 'types/cubeMeta';
import { FilterType, IDimensionQuery } from 'types/query';
import DefaultSelectableDimensionSelection from './DefaultSelectableDimensionSelection';
import { QueryContext } from '../../contexts/queryContext';
import ComputedValuesButton from './ComputedValues/ComputedValuesButton';
import { areDimensionSelectionPropsEqual, IDimensionSelectionProps } from './dimensionSelectionProps';

const ComponentWrapper = styled(Stack)`
    justify-content: space-between;
    align-items: flex-start;
    width: 100%;
`;

const ToolbarWrapper = styled(Stack)`
    width: 100%;
    align-items: center;
`;

const FilterMethodControl = styled(FormControl)`
    flex-basis: 0;
    flex-grow: 1;
    min-width: 0;
`;

const SelectableSettingsContent = styled(Stack)`
    box-sizing: border-box;
    padding: 16px;
    width: min(360px, calc(100vw - 32px));
`;

export const DimensionSelection: React.FC<IDimensionSelectionProps> = ({ dimension, resolvedDimensionValueCodes, query }) => {
    const { t } = useTranslation();
    const { setQuery } = React.useContext(QueryContext);
    const [selectabilityAnchorEl, setSelectabilityAnchorEl] = React.useState<HTMLElement | null>(null);
    const dimensionQuery = query[dimension.code];
    const selectabilityPopoverId = `selectable-settings-popover-${dimension.code}`;
    const selectabilityHeadingId = `selectable-settings-heading-${dimension.code}`;

    const onQueryChanged = (newQuery: IDimensionQuery) => {
        setQuery(currentQuery => ({
            ...(currentQuery ?? query),
            [dimension.code]: newQuery
        }));
    };

    const handleFilterTypeChanged = (event: SelectChangeEvent<FilterType>) => {
        const newFilterType = event.target.value as FilterType;
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

    let filterComponent = null;
    let selectedValues = null;

    switch (dimensionQuery.valueFilter.type) {
        case FilterType.Item:
            // NOTE: this selectedValues resolution block is duplicated in the FilterType.InverseItem
            // case below - keep both in sync (or extract to a helper) if this logic changes.
            if (dimensionQuery?.valueFilter?.query && dimension?.values) {
                const stringArray = dimensionQuery.valueFilter.query as string[];
                selectedValues = stringArray
                    .map(code => dimension.values.find(o => o.code === code))
                    .filter((value): value is IDimensionValue => value !== undefined);
            }
            filterComponent =
                <ManualPickDimensionSelection
                    options={dimension.values}
                    selectedValues={selectedValues}
                    onQueryChanged={handleFilterValueChanged}
                />
            break;
        case FilterType.All:
            // The active filter method is already shown in the filter method selector above;
            // no additional input is needed for the "All" method.
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
        case FilterType.InverseItem:
            if (dimensionQuery?.valueFilter?.query && dimension?.values) {
                const stringArray = dimensionQuery.valueFilter.query as string[];
                selectedValues = stringArray
                    .map(code => dimension.values.find(o => o.code === code))
                    .filter((value): value is IDimensionValue => value !== undefined);
            }
            filterComponent =
                <ManualPickDimensionSelection
                    options={dimension.values}
                    selectedValues={selectedValues}
                    onQueryChanged={handleFilterValueChanged}
                    label="variableSelect.excludedValuesLabel"
                />
            break;
        case FilterType.Regex:
            filterComponent =
                <RegexDimensionSelection
                    pattern={dimensionQuery.valueFilter.query as string}
                    onQueryChanged={handleFilterValueChanged}
                />
            break;
    }

    return (
        <ComponentWrapper direction="column" spacing={2}>
            <ToolbarWrapper direction="row" spacing={1}>
                <FilterMethodControl size="small">
                    <InputLabel id={`filter-method-label-${dimension.code}`}>{t('variableSelect.filterMethodLabel')}</InputLabel>
                    <Select
                        labelId={`filter-method-label-${dimension.code}`}
                        label={t('variableSelect.filterMethodLabel')}
                        value={dimensionQuery.valueFilter.type}
                        onChange={handleFilterTypeChanged}
                    >
                        {
                            (Object.values(FilterType) as Array<FilterType>).map(queryType => (
                                <MenuItem key={queryType} value={queryType}>
                                    {t(queryTypeLabels[queryType])}
                                </MenuItem>
                            ))
                        }
                    </Select>
                </FilterMethodControl>
                <ComputedValuesButton dimension={dimension} dimensionQuery={dimensionQuery} onQueryChanged={onQueryChanged} />
                <Tooltip title={t(dimensionQuery.selectable ? 'variableSelect.selectableSettingsActive' : 'variableSelect.selectableSettings')}>
                    <IconButton
                        aria-label={t(dimensionQuery.selectable ? 'variableSelect.selectableSettingsActive' : 'variableSelect.selectableSettings')}
                        aria-haspopup="dialog"
                        aria-expanded={selectabilityAnchorEl !== null}
                        aria-controls={selectabilityAnchorEl !== null ? selectabilityPopoverId : undefined}
                        color={dimensionQuery.selectable ? 'primary' : 'default'}
                        size="small"
                        onClick={(event) => setSelectabilityAnchorEl(event.currentTarget)}
                    >
                        <Badge
                            anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                            badgeContent={dimensionQuery.selectable ? <CheckIcon data-testid="selectability-active-marker" sx={{ fontSize: 10 }} /> : undefined}
                            color="primary"
                            overlap="circular"
                            sx={{ '& .MuiBadge-badge': { height: 14, minWidth: 14, padding: 0 } }}
                        >
                            <TouchAppIcon />
                        </Badge>
                    </IconButton>
                </Tooltip>
            </ToolbarWrapper>

            {filterComponent}

            {
                dimensionQuery.valueFilter.type === FilterType.Item ? null : (
                    <ResultList dimensionValues={dimension.values} resolvedDimensionValueCodes={resolvedDimensionValueCodes} />
                )
            }

            <Popover
                open={selectabilityAnchorEl !== null}
                anchorEl={selectabilityAnchorEl}
                onClose={() => setSelectabilityAnchorEl(null)}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                transformOrigin={{ vertical: 'top', horizontal: 'right' }}
                slotProps={{
                    paper: {
                        id: selectabilityPopoverId,
                        role: 'dialog',
                        'aria-labelledby': selectabilityHeadingId,
                    }
                }}
            >
                <SelectableSettingsContent spacing={1}>
                    <Typography id={selectabilityHeadingId} component="h3" variant="subtitle2">{t('variableSelect.selectableSettings')}</Typography>
                    <SelectabilitySwitch autoFocus onChange={value => onQueryChanged({ ...dimensionQuery, selectable: value })} selected={dimensionQuery.selectable} />
                    {
                        dimensionQuery.selectable && (
                            <DefaultSelectableDimensionSelection dimensionCode={dimension.code} resolvedDimensionValueCodes={resolvedDimensionValueCodes} options={dimension.values} />
                        )
                    }
                </SelectableSettingsContent>
            </Popover>
        </ComponentWrapper>
    );
}
export default React.memo(DimensionSelection, areDimensionSelectionPropsEqual);