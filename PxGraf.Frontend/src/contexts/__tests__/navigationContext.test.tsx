import { NavigationProvider, useNavigationContext } from "contexts/navigationContext";
import { act, fireEvent, render } from '@testing-library/react';
import '@testing-library/jest-dom';

const TestComponent = () => {
    const { tablePath, setTablePath } = useNavigationContext();

    return (
        <>
            <div>{tablePath ? tablePath.join(',') : 'null'}</div>
            <button data-testid='button1' onClick={() => setTablePath(['testDb', 'testStat', 'testTable'])}>Update state</button>
        </>
    );
}

describe('navigationContext', () => {
    it('should have null values initially', () => {
        const { getByText } = render(<NavigationProvider><TestComponent /></NavigationProvider>);
        expect(getByText('null')).toBeInTheDocument();
    });

    it('should change values correctly', async() => {
        const { getByText, getByTestId } = render(<NavigationProvider><TestComponent /></NavigationProvider>);

        const button1 = getByTestId('button1');

        act(() => {
            fireEvent.click(button1);
        });
        
        expect(getByText('testDb,testStat,testTable')).toBeInTheDocument(); 
    });
});
