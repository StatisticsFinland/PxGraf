import { IMetaProperty } from "../types/cubeMeta";
import { getAdditionalProperty } from "./metadataUtils";

const mockProperties: { [key: string]: IMetaProperty } = {
    MOCKPROPERTY_A: {
        KeyWord: 'MOCKPROPERTY_A',
        CanGetStringValue: true,
        CanGetMultilanguageValue: false,
        Entries: 'Mock property A value'
    },
    MOCKPROPERTY_B: {
        KeyWord: 'MOCKPROPERTY_B',
        CanGetStringValue: false,
        CanGetMultilanguageValue: true,
        Entries: {
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        }
    }
}

describe('getAdditionalProperty tests', () => {
    it('Should return the correct property value as string', () => {
        const propertyValue = getAdditionalProperty('MOCKPROPERTY_A', mockProperties, true);
        expect(propertyValue).toBe('Mock property A value');
    });

    it('Should return the correct property value as multilanguage string', () => {
        const propertyValue = getAdditionalProperty('MOCKPROPERTY_B', mockProperties);
        expect(propertyValue).toEqual({
            fi: 'Mock property B value in Finnish',
            sv: 'Mock property B value in Swedish',
            en: 'Mock property B value in English'
        });
    });

    it('Should return null when property is not found', () => {
        const propertyValue = getAdditionalProperty('foo', mockProperties);
        expect(propertyValue).toBeNull();
    });
});