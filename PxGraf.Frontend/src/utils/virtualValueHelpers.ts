import { FilterType, IDimensionQuery, IVirtualValueDefinition, VirtualValueOperator } from '../types/query';

const CODE_SUFFIX_PATTERN = /_(\d+)$/;

/**
 * Generates the next sequential virtual value code for the given operator type.
 * Produces "sum_1", "subtraction_1", "multiplication_1", "division_1", "sum_2", etc.
 * The counter is per operator type: each type has its own independent sequence.
 */
export function generateVirtualValueCode(existing: IVirtualValueDefinition[], operator: VirtualValueOperator): string {
    let max = 0;
    for (const def of existing) {
        if (getOperatorType(def) !== operator) continue;
        const match = CODE_SUFFIX_PATTERN.exec(def.code);
        if (match) {
            const n = Number.parseInt(match[1], 10);
            if (n > max) {
                max = n;
            }
        }
    }
    return `${operator}_${max + 1}`;
}

/**
 * Returns true if the definitions have no circular dependencies.
 * realValueCodes provides the set of known non-virtual value codes.
 */
export function validateNoCycles(
    definitions: IVirtualValueDefinition[],
    realValueCodes: string[]
): boolean {
    const virtualCodes = new Set(definitions.map(d => d.code));
    const realSet = new Set(realValueCodes);

    // Build adjacency: virtualCode → virtual operand codes it depends on
    const graph = new Map<string, string[]>();
    for (const def of definitions) {
        const virtualDeps = getOperandCodes(def).filter(c => virtualCodes.has(c) && !realSet.has(c));
        graph.set(def.code, virtualDeps);
    }

    const visited = new Set<string>();
    const recursionStack = new Set<string>();

    function hasCycle(node: string): boolean {
        if (recursionStack.has(node)) {
            return true;
        }
        if (visited.has(node)) {
            return false;
        }
        visited.add(node);
        recursionStack.add(node);
        const neighbours = graph.get(node) ?? [];
        for (const neighbour of neighbours) {
            if (hasCycle(neighbour)) {
                return true;
            }
        }
        recursionStack.delete(node);
        return false;
    }

    for (const code of virtualCodes) {
        if (hasCycle(code)) {
            return false;
        }
    }
    return true;
}

/**
 * Builds a placeholder multilanguage name object for a new virtual value.
 * Returns { "en": "<translation> N", ... } for all provided languages.
 */
export function generatePlaceholderNames(
    availableLanguages: string[],
    translateForLang: (lang: string) => string,
    sequentialNumber: number
): Record<string, string> {
    const result: Record<string, string> = {};
    for (const lang of availableLanguages) {
        result[lang] = `${translateForLang(lang)} ${sequentialNumber}`;
    }
    return result;
}

/**
 * Returns all operand codes for the given virtual value definition.
 */
export function getOperandCodes(def: IVirtualValueDefinition): string[] {
    switch (def.type) {
        case 'sum': return def.operandCodes;
        case 'subtractionOfTwo': return [def.minuend, def.subtrahend];
        case 'subtractionByConstant': return [def.operand];
        case 'multiplicationOfTwo': return [def.leftOperand, def.rightOperand];
        case 'multiplicationByConstant': return [def.operand];
        case 'divisionOfTwo': return [def.dividend, def.divisor];
        case 'divisionByConstant': return [def.operand];
    }
}

/**
 * Returns the operator type for the given virtual value definition.
 */
export function getOperatorType(def: IVirtualValueDefinition): VirtualValueOperator {
    switch (def.type) {
        case 'sum': return 'sum';
        case 'subtractionOfTwo':
        case 'subtractionByConstant': return 'subtraction';
        case 'multiplicationOfTwo':
        case 'multiplicationByConstant': return 'multiplication';
        case 'divisionOfTwo':
        case 'divisionByConstant': return 'division';
    }
}

/**
 * Returns the constant operand for the given virtual value definition, if any.
 */
export function getConstant(def: IVirtualValueDefinition): number | undefined {
    switch (def.type) {
        case 'sum': return def.constant;
        case 'subtractionByConstant':
        case 'multiplicationByConstant':
        case 'divisionByConstant': return def.constant;
        default: return undefined;
    }
}

/**
 * Builds a typed virtual value definition from the given form fields.
 */
export function buildDefinition(
    code: string,
    operator: VirtualValueOperator,
    operandCodes: string[],
    constant: number | undefined,
): IVirtualValueDefinition {
    switch (operator) {
        case 'sum':
            return { type: 'sum', code, operandCodes, constant };
        case 'subtraction':
            return constant !== undefined
                ? { type: 'subtractionByConstant', code, operand: operandCodes[0], constant }
                : { type: 'subtractionOfTwo', code, minuend: operandCodes[0], subtrahend: operandCodes[1] };
        case 'multiplication':
            return constant !== undefined
                ? { type: 'multiplicationByConstant', code, operand: operandCodes[0], constant }
                : { type: 'multiplicationOfTwo', code, leftOperand: operandCodes[0], rightOperand: operandCodes[1] };
        case 'division':
            return constant !== undefined
                ? { type: 'divisionByConstant', code, operand: operandCodes[0], constant }
                : { type: 'divisionOfTwo', code, dividend: operandCodes[0], divisor: operandCodes[1] };
    }
}

/**
 * Builds placeholder unitEdit and sourceEdit objects for a new virtual content dimension value.
 * Returns objects mapping each available language to the translation for that language.
 */
export function generatePlaceholderContentEdits(
    availableLanguages: string[],
    translateForLang: (lang: string) => string
): { unitEdit: Record<string, string>; sourceEdit: Record<string, string> } {
    const placeholder: Record<string, string> = {};
    for (const lang of availableLanguages) {
        placeholder[lang] = translateForLang(lang);
    }
    return { unitEdit: { ...placeholder }, sourceEdit: { ...placeholder } };
}

/**
 * Removes from a FilterType.Item query any virtual codes that were deleted between
 * previousDefinitions and nextDimensionQuery.virtualValueDefinitions.
 * Returns the input query unchanged if no cleanup is needed.
 */
export function removeDeletedVirtualCodesFromItemFilter(
    previousDefinitions: IVirtualValueDefinition[],
    nextDimensionQuery: IDimensionQuery,
): IDimensionQuery {
    const nextCodes = new Set(nextDimensionQuery.virtualValueDefinitions.map(d => d.code));
    const deletedCodes = previousDefinitions
        .filter(d => !nextCodes.has(d.code))
        .map(d => d.code);

    if (deletedCodes.length === 0) {
        return nextDimensionQuery;
    }

    const { valueFilter } = nextDimensionQuery;
    if (valueFilter.type !== FilterType.Item || !valueFilter.query) {
        return nextDimensionQuery;
    }

    const cleanedQuery = (valueFilter.query as string[]).filter(
        code => !deletedCodes.includes(code),
    );

    return {
        ...nextDimensionQuery,
        valueFilter: {
            ...valueFilter,
            query: cleanedQuery,
        },
    };
}
