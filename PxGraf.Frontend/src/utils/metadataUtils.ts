import { EMetaPropertyType, IMetaProperty } from "../types/cubeMeta";
import { MultiLanguageString } from "../types/multiLanguageString";

/**
 * Gets the value of a metadata property by keyword.
 * @param key - The keyword for the property to search for
 * @param additionalProperties - All meta properties to be searched
 * @returns Null if property with the given key is not found. Otherwise the value of the meta property. Type depends on the type of the meta property.
 */
export function getAdditionalPropertyValue(key: string, additionalProperties: { [key: string]: IMetaProperty }): string | MultiLanguageString | boolean | number | string[] | MultiLanguageString[] {
    if (!additionalProperties) return null;

    if (additionalProperties[key]) {
        switch (additionalProperties[key].Type) {
            case EMetaPropertyType.Text:
                return additionalProperties[key].Value as string;
            case EMetaPropertyType.MultilanguageText:
                return additionalProperties[key].Value as MultiLanguageString;
            case EMetaPropertyType.Numeric:
                return additionalProperties[key].Value as number;
            case EMetaPropertyType.Boolean:
                return additionalProperties[key].Value as boolean;
            case EMetaPropertyType.TextArray:
                return additionalProperties[key].Value as string[];
            case EMetaPropertyType.MultilanguageTextArray:
                return additionalProperties[key].Value as MultiLanguageString[];
        }
    }
    return null;
}
