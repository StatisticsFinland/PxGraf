import { EMetaPropertyType, IMetaProperty } from "../types/cubeMeta";
import { getAdditionalPropertyValue } from "./metadataUtils";

const mockProperties: { [key: string]: IMetaProperty } = {
    MOCKPROPERTY_A: {
        Type: EMetaPropertyType.Text,
        Value: 'Mock property A value'
    },
    MOCKPROPERTY_B: {
        Type: EMetaPropertyType.MultilanguageText,
        Value: {
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        }
    }
}

describe('getAdditionalProperty tests', () => {
    it('Should return the correct property value as string', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_A', mockProperties);
        expect(propertyValue).toBe('Mock property A value');
    });

    it('Should return the correct property value as multilanguage string', () => {
        const propertyValue = getAdditionalPropertyValue('MOCKPROPERTY_B', mockProperties);
        expect(propertyValue).toEqual({
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        });
    });

    it('Should return null when property is not found', () => {
        const propertyValue = getAdditionalPropertyValue('foo', mockProperties);
        expect(propertyValue).toBeNull();
    });
});