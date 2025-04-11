import { TextEncoder, TextDecoder } from 'util';

global.TextEncoder = TextEncoder as typeof global.TextEncoder;
global.TextDecoder = TextDecoder as typeof global.TextDecoder;

if (!window.CSS) window.CSS = {} as any;
if (!window.CSS.supports) window.CSS.supports = () => true;