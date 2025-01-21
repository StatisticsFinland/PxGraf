import { EMetaPropertyType, IMetaProperty } from "../types/cubeMeta";
import { getAdditionalPropertyValue } from "./metadataUtils";

const mockProperties: { [key: string]: IMetaProperty } = {
    MOCKPROPERTY_STRING: {
        type: EMetaPropertyType.Text,
        value: 'Mock property A value'
    },
    MOCKPROPERTY_MULTILANGUAGESTRING: {
        type: EMetaPropertyType.MultilanguageText,
        value: {
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        }
    },
    MOCKPROPERTY_NUMBER: {
        type: EMetaPropertyType.Numeric,
        value: 42
    },
    MOCKPROPERTY_BOOLEAN: {
        type: EMetaPropertyType.Boolean,
        value: true
    },
    MOCKPROPERTY_STRING_ARRAY: {
        type: EMetaPropertyType.TextArray,
        value: ['foo', 'bar', 'baz']
    },
    MOCKPROPERTY_MULTILANGUAGESTRING_ARRAY: {
        type: EMetaPropertyType.MultilanguageTextArray,
        value: [
            {
                fi: 'Foo in Finnish',
                sv: 'Foo in Swedish',
                en: 'Foo in English'
            },
            {
                fi: 'Bar in Finnish',
                sv: 'Bar in Swedish',
                en: 'Bar in English'
            },
            {
                fi: 'Baz in Finnish',
                sv: 'Baz in Swedish',
                en: 'Baz in English'
            }
        ]
    }
}

describe('getAdditionalProperty tests', () => {
    it('Should return the correct property value as string', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_STRING', mockProperties);
        expect(propertyValue).toBe('Mock property A value');
    });

    it('Should return the correct property value as multilanguage string', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_MULTILANGUAGESTRING', mockProperties);
        expect(propertyValue).toEqual({
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        });
    });

    it('Should return the correct property value as number', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_NUMBER', mockProperties);
        expect(propertyValue).toBe(42);
    });

    it('Should return the correct property value as boolean', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_BOOLEAN', mockProperties);
        expect(propertyValue).toBe(true);
    });

    it('Should return the correct property value as string array', () => {
            const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_STRING_ARRAY', mockProperties);
        expect(propertyValue).toEqual(['foo', 'bar', 'baz']);
    });

    it('Should return the correct property value as multilanguage string array', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_MULTILANGUAGESTRING_ARRAY', mockProperties);
        expect(propertyValue).toEqual([
            {
                fi: 'Foo in Finnish',
                sv: 'Foo in Swedish',
                en: 'Foo in English'
            },
            {
                fi: 'Bar in Finnish',
                sv: 'Bar in Swedish',
                en: 'Bar in English'
            },
            {
                fi: 'Baz in Finnish',
                sv: 'Baz in Swedish',
                en: 'Baz in English'
            }
        ]);
    });

    it('Should return null when property is not found', () => {
        const propertyValue = getAdditionalPropertyValue('foo', mockProperties);
        expect(propertyValue).toBeNull();
    });
});