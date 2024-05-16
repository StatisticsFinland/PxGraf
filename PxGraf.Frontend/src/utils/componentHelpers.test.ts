import { a11yProps, spacing } from "./componentHelpers";

describe('spacing tests', () => {
    it('Should return the correct object', () => {
        const result = spacing(5);
        expect(result).toBeTruthy();
        expect(result).toEqual({ '*:not(style) + &': { mt: 5 }, '&:not(:last-child)': { mb: 5 } });
    });
});

describe('a11yProps tests', () => {
    it('Should return the correct object', () => {
        const result = a11yProps('seppo');
        expect(result).toBeTruthy();
        expect(result).toEqual({ id: 'simple-tab-seppo', 'aria-controls': 'simple-tabpanel-seppo' });
    });
});