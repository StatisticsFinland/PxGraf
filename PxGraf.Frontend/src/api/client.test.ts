import ApiClient from './client';

describe('API client mode isolation', () => {
    const flags = globalThis as typeof globalThis & { __PXGRAF_STANDALONE__?: boolean };
    const originalFlag = flags.__PXGRAF_STANDALONE__;
    const originalFetch = globalThis.fetch;

    afterEach(() => {
        flags.__PXGRAF_STANDALONE__ = originalFlag;
        globalThis.fetch = originalFetch;
    });

    it('preserves normal network errors without activating standalone responses', async () => {
        flags.__PXGRAF_STANDALONE__ = false;
        const fetchMock = jest.fn().mockRejectedValue(new Error('Backend unavailable'));
        globalThis.fetch = fetchMock;
        await expect(new ApiClient().getAsync('creation/data-bases/')).rejects.toThrow('Backend unavailable');
        expect(fetchMock).toHaveBeenCalledTimes(1);
    });

    it('never fetches a backend response while standalone is enabled', async () => {
        flags.__PXGRAF_STANDALONE__ = true;
        const fetchMock = jest.fn();
        globalThis.fetch = fetchMock;
        const result = await new ApiClient().getAsync('creation/data-bases/');
        expect(result).toHaveProperty('headers.0.code', 'standalone');
        await expect(new ApiClient().getAsync('creation/missing')).rejects.toThrow('Unsupported standalone request');
        expect(fetchMock).not.toHaveBeenCalled();
    });
});