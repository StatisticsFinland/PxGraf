import { IMetaProperty } from "../types/cubeMeta";
import { MultiLanguageString } from "../types/multiLanguageString";

export function getAdditionalProperty(key: string, additionalProperties: { [key: string]: IMetaProperty }, forceToString = false): string | MultiLanguageString {
    if (!additionalProperties) return null;

    if (additionalProperties[key]) {
        if (additionalProperties[key].CanGetStringValue) {
            return additionalProperties[key].Entries as string;
        }
        else if (additionalProperties[key].CanGetMultilanguageValue) {
            if (forceToString) return additionalProperties[key].Entries[0];
            else return additionalProperties[key].Entries;
        }
    }
    return null;
}
