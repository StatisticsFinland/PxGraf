import { IDimension } from 'types/cubeMeta';
import { Query } from 'types/query';

export interface IDimensionSelectionProps {
    dimension: IDimension;
    resolvedDimensionValueCodes: string[];
    query: Query;
}

export const areDimensionSelectionPropsEqual = (previous: IDimensionSelectionProps, next: IDimensionSelectionProps) => {
    // Normalize to empty arrays so undefined/null (e.g. codes not yet resolved) never causes a
    // false "not equal" result and an unnecessary rerender.
    const previousCodes = previous.resolvedDimensionValueCodes ?? [];
    const nextCodes = next.resolvedDimensionValueCodes ?? [];
    const resolvedCodesEqual = previousCodes === nextCodes || (
        previousCodes.length === nextCodes.length
        && previousCodes.every((code, index) => code === nextCodes[index])
    );

    return previous.dimension === next.dimension
        && previous.query[previous.dimension.code] === next.query[next.dimension.code]
        && resolvedCodesEqual;
};
