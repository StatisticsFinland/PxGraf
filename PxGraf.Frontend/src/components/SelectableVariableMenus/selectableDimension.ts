import { IVariable } from 'types/visualizationResponse';

type ISelectableDimensionValue = Pick<IVariable['values'][number], 'code' | 'name'> &
    Partial<Omit<IVariable['values'][number], 'code' | 'name'>>;

export type ISelectableDimension = Pick<IVariable, 'code' | 'name'> &
    Partial<Omit<IVariable, 'code' | 'name' | 'values'>> & {
    values: ISelectableDimensionValue[];
};

export interface ISelectabilityInfo {
    dimension: ISelectableDimension;
}