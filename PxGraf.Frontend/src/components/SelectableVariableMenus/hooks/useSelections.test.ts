import { renderHook, act } from '@testing-library/react';
import useSelections from './useSelections';

describe('useSelections', () => {
    it('initializes with empty selections by default', () => {
        const { result } = renderHook(() => useSelections());
        expect(result.current.selections).toEqual({});
    });

    it('initializes with provided initial selections', () => {
        const initial = { foo: ['a', 'b'], bar: ['c'] };
        const { result } = renderHook(() => useSelections(initial));
        expect(result.current.selections).toEqual(initial);
    });

    it('updates selections via setSelections', () => {
        const { result } = renderHook(() => useSelections());
        act(() => {
            result.current.setSelections({ dimension1: ['val1', 'val2'] });
        });
        expect(result.current.selections).toEqual({ dimension1: ['val1', 'val2'] });
    });

    it('replaces all selections when setSelections is called', () => {
        const { result } = renderHook(() => useSelections({ foo: ['a'] }));
        act(() => {
            result.current.setSelections({ bar: ['b'] });
        });
        expect(result.current.selections).toEqual({ bar: ['b'] });
        expect(result.current.selections).not.toHaveProperty('foo');
    });

    it('supports functional updates via setSelections', () => {
        const { result } = renderHook(() => useSelections({ foo: ['a'] }));
        act(() => {
            result.current.setSelections(prev => ({ ...prev, bar: ['b'] }));
        });
        expect(result.current.selections).toEqual({ foo: ['a'], bar: ['b'] });
    });

    it('can clear all selections by setting empty object', () => {
        const { result } = renderHook(() => useSelections({ foo: ['a'], bar: ['b'] }));
        act(() => {
            result.current.setSelections({});
        });
        expect(result.current.selections).toEqual({});
    });
});
