import { TextEncoder, TextDecoder } from 'node:util';
import { ReadableStream, TransformStream, WritableStream } from 'node:stream/web';
import { MessageChannel, MessagePort } from 'node:worker_threads';
import { serialize, deserialize } from 'node:v8';

globalThis.TextEncoder = TextEncoder;
globalThis.TextDecoder = TextDecoder;
globalThis.structuredClone ??= <T>(val: T): T => deserialize(serialize(val));

// jsdom v28+ depends on undici which expects web stream APIs in the global scope
globalThis.ReadableStream ??= ReadableStream as typeof globalThis.ReadableStream;
globalThis.WritableStream ??= WritableStream as typeof globalThis.WritableStream;
globalThis.TransformStream ??= TransformStream as typeof globalThis.TransformStream;
// Override MessageChannel with node:worker_threads version + auto-unref on ports
// to prevent React scheduler's internal MessageChannel from keeping the Jest worker alive.
// jsdom's built-in MessageChannel lacks unref(), so we always replace it.
// Setting onmessage implicitly calls ref(), so we patch the setter to re-unref.
{
    const patchPort = (port: MessagePort) => {
        const originalDescriptor = Object.getOwnPropertyDescriptor(Object.getPrototypeOf(port), 'onmessage');
        if (originalDescriptor?.set) {
            const setter = originalDescriptor.set;
            Object.defineProperty(port, 'onmessage', {
                get: originalDescriptor.get?.bind(port),
                set(handler) {
                    setter.call(port, handler);
                    port.unref();
                },
                configurable: true,
            });
        }
    };

    class UnrefMessageChannel extends MessageChannel {
        constructor() {
            super();
            patchPort(this.port1);
            patchPort(this.port2);
        }
    }
    globalThis.MessageChannel = UnrefMessageChannel as typeof globalThis.MessageChannel;
}
globalThis.MessagePort ??= MessagePort as typeof globalThis.MessagePort;

if (!globalThis.CSS) globalThis.CSS = {} as any;
if (!globalThis.CSS.supports) globalThis.CSS.supports = () => true;

// Mock envVars to handle import.meta.env in Jest
jest.mock('envVars', () => ({
    PxGrafUrl: 'http://localhost:3000',
    PublicUrl: '',
    BasePath: ''
}));

// Centralized react-i18next mock for all test files
jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => ({
        t: (str: string) => str,
        i18n: {
            changeLanguage: () => Promise.resolve(),
        },
    }),
}));