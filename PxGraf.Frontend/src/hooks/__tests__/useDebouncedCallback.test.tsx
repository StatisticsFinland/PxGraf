import React from 'react';
import { act, render } from '@testing-library/react';
import useDebouncedCallback from 'hooks/useDebouncedCallback';

beforeEach(() => {
    jest.useFakeTimers();
});

afterEach(() => {
    jest.useRealTimers();
});

interface ITestComponentProps {
    callback: (value: string) => void;
}

const TestComponent: React.FC<ITestComponentProps> = ({ callback }) => {
    const debounced = useDebouncedCallback(callback, 500);
    return <button onClick={() => debounced('clicked')} data-testid="trigger" />;
};

describe('useDebouncedCallback', () => {
    it('does not call the callback before the delay has elapsed', () => {
        const callback = jest.fn();
        const { getByTestId } = render(<TestComponent callback={callback} />);

        act(() => {
            getByTestId('trigger').click();
        });

        expect(callback).not.toHaveBeenCalled();
    });

    it('calls the callback with the latest arguments after the delay elapses', () => {
        const callback = jest.fn();
        const { getByTestId } = render(<TestComponent callback={callback} />);

        act(() => {
            getByTestId('trigger').click();
        });
        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(callback).toHaveBeenCalledTimes(1);
        expect(callback).toHaveBeenCalledWith('clicked');
    });

    it('only invokes once for rapid calls within the delay window', () => {
        const callback = jest.fn();
        const { getByTestId } = render(<TestComponent callback={callback} />);

        act(() => {
            getByTestId('trigger').click();
            getByTestId('trigger').click();
            getByTestId('trigger').click();
        });
        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(callback).toHaveBeenCalledTimes(1);
    });

    it('still invokes the latest callback even if the callback identity changes while pending', () => {
        const firstCallback = jest.fn();
        const secondCallback = jest.fn();
        const { getByTestId, rerender } = render(<TestComponent callback={firstCallback} />);

        act(() => {
            getByTestId('trigger').click();
        });

        rerender(<TestComponent callback={secondCallback} />);

        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(firstCallback).not.toHaveBeenCalled();
        expect(secondCallback).toHaveBeenCalledWith('clicked');
    });

    it('cancels a pending call on unmount', () => {
        const callback = jest.fn();
        const { getByTestId, unmount } = render(<TestComponent callback={callback} />);

        act(() => {
            getByTestId('trigger').click();
        });

        unmount();

        act(() => {
            jest.advanceTimersByTime(500);
        });

        expect(callback).not.toHaveBeenCalled();
    });
});
