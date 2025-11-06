import { TextEncoder, TextDecoder } from 'util';

global.TextEncoder = TextEncoder as typeof global.TextEncoder;
global.TextDecoder = TextDecoder as typeof global.TextDecoder;

if (!window.CSS) window.CSS = {} as any;
if (!window.CSS.supports) window.CSS.supports = () => true;

// Mock envVars to handle import.meta.env in Jest
jest.mock('envVars', () => ({
    PxGrafUrl: 'http://localhost:3000',
    PublicUrl: '',
    BasePath: ''
}));