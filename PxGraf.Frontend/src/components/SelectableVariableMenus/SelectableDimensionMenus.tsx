import { Stack } from '@mui/material';
import { ValueSelect } from './ValueSelect';
import React, { Dispatch, SetStateAction } from 'react';
import { ISelectabilityInfo } from 'components/Preview/Preview';

export interface ISelectableSelections {
    [key: string]: string[];
}

interface ISelectableDimensionMenusProps {
    selectables: ISelectabilityInfo[];
    setSelections: Dispatch<SetStateAction<ISelectableSelections>>;
    selections: ISelectableSelections;
    multiselectableDimensionCode?: string;
}

export const SelectableDimensionMenus: React.FC<ISelectableDimensionMenusProps> = ({ selectables, setSelections, selections, multiselectableDimensionCode }) => {
    return (
        <Stack direction="row" spacing={2}>
            {selectables?.map(v => {
                return <ValueSelect
                    key={v.dimension.code}
                    dimension={v.dimension}
                    multiselect={multiselectableDimensionCode == v.dimension.code}
                    activeSelections={selections[v.dimension.code] ?? []}
                    onValueChanged={value => {
                        if (value != null && value.length > 0) {
                            setSelections({
                                ...selections,
                                [v.dimension.code]: typeof value === 'string'
                                    ? value.split(',')
                                    : value
                            })
                        }
                    }}
                />
            })}
        </Stack>
    )
}

export default SelectableDimensionMenus;