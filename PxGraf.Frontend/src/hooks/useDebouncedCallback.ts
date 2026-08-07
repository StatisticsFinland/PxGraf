import { useEffect, useMemo, useRef } from 'react';
import { debounce, DebouncedFunc } from 'lodash';

/**
 * Returns a debounced wrapper around `callback`. The debounce timer has a stable
 * identity across re-renders, so a pending call is not dropped if `callback`
 * changes identity while a delay is in progress - the latest `callback` is
 * always the one invoked when the delay elapses. Any pending call is cancelled
 * on unmount.
 */
const useDebouncedCallback = <Args extends unknown[]>(
    callback: (...args: Args) => void,
    delayMs: number
): DebouncedFunc<(...args: Args) => void> => {
    const callbackRef = useRef(callback);

    useEffect(() => {
        callbackRef.current = callback;
    }, [callback]);

    const debounced = useMemo(
        // eslint-disable-next-line react-hooks/refs -- intentional "latest ref" pattern: only read inside the debounced/timer callback, which always runs outside of render
        () => debounce((...args: Args) => callbackRef.current(...args), delayMs),
        [delayMs]
    );

    useEffect(() => {
        // NOTE: cancels rather than flushes, so an edit made within the delay window is lost if the
        // component unmounts before it fires. This matches the existing cubeQuery debounce behavior.
        return () => {
            debounced.cancel();
        };
    }, [debounced]);

    return debounced;
};

export default useDebouncedCallback;
