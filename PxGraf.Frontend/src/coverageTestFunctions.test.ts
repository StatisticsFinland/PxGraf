import { coveredTestFunction } from './coverageTestFunctions';

describe('coverage tests', () => {
    it('should return right number', () => {
        const result = coveredTestFunction(5);
        expect(result).toEqual(10);
    });
});