import { EMetaPropertyType, IMetaProperty } from "../types/cubeMeta";
import { MultiLanguageString } from "../types/multiLanguageString";

export function getAdditionalPropertyValue(key: string, additionalProperties: { [key: string]: IMetaProperty }): string | MultiLanguageString | boolean | number | string[] | MultiLanguageString[] {
    if (!additionalProperties) return null;

    if (additionalProperties[key]) {
        switch (additionalProperties[key].Type) {
            case EMetaPropertyType.Text:
                return additionalProperties[key].Value as string;
            case EMetaPropertyType.MultilanguageText:
                return additionalProperties[key].Value as MultiLanguageString;
            case EMetaPropertyType.Numeric: // Numeric
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
