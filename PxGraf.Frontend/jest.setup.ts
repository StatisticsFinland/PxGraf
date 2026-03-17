import { TextEncoder, TextDecoder } from 'util';
import { ReadableStream, TransformStream, WritableStream } from 'stream/web';
import { MessageChannel, MessagePort } from 'worker_threads';

global.TextEncoder = TextEncoder as typeof global.TextEncoder;
global.TextDecoder = TextDecoder as typeof global.TextDecoder;

// jsdom v28+ depends on undici which expects web stream APIs in the global scope
if (typeof globalThis.ReadableStream === 'undefined') {
    globalThis.ReadableStream = ReadableStream as typeof globalThis.ReadableStream;
}
if (typeof globalThis.WritableStream === 'undefined') {
    globalThis.WritableStream = WritableStream as typeof globalThis.WritableStream;
}
if (typeof globalThis.TransformStream === 'undefined') {
    globalThis.TransformStream = TransformStream as typeof globalThis.TransformStream;
}
if (typeof globalThis.MessageChannel === 'undefined') {
    globalThis.MessageChannel = MessageChannel as typeof globalThis.MessageChannel;
}
if (typeof globalThis.MessagePort === 'undefined') {
    globalThis.MessagePort = MessagePort as typeof globalThis.MessagePort;
}

if (!window.CSS) window.CSS = {} as any;
if (!window.CSS.supports) window.CSS.supports = () => true;

// Mock envVars to handle import.meta.env in Jest
jest.mock('envVars', () => ({
    PxGrafUrl: 'http://localhost:3000',
    PublicUrl: '',
    BasePath: ''
}));