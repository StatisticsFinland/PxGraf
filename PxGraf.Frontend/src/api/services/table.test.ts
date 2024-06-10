import { sortTableData, ITableListResponse } from './table';

const mockTableData: ITableListResponse[] = [
{
        id: '0',
        type: 't',
        updated: '2024-6-10',
        text: { en: 'Foo-en', fi: 'Foo-fi' },
        languages: ['en', 'fi'],
    },
    {
        id: '1',
        type: 't',
        updated: '2021-6-10',
        text: { en: 'Bar-en', fi: 'Bar-fi' },
        languages: ['en', 'fi'],
    },
    {
        id: '2',
        type: 't',
        updated: '2021-6-10',
        text: { fi: 'Baz-fi' },
        languages: ['fi'],
    },
];

const mockPrimaryLanguage = 'en';

describe('sortTableData', () => {
    it('should sort table data by primary or first available language', () => {
        const sortedData = sortTableData(mockTableData, mockPrimaryLanguage);
        const expected = ["1", "2", "0"];
        const result = sortedData.map((item) => item.id);
        expect(result).toEqual(expected);
    });
});