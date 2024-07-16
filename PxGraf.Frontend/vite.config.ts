import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'

// https://vitejs.dev/config/
export default defineConfig({
    base: '/',
    plugins: [react()],
    resolve: {
        alias: {
            'Router': '/src/Router.tsx',
            'contexts': '/src/contexts',
            'types': '/src/types',
            'styles': '/src/styles',
            'views': '/src/views',
            'components': '/src/components',
            'hooks': '/src/hooks',
            'api': '/src/api',
            'utils': '/src/utils',
            'images': '/src/images',
            'dummy': '/src/dummy',
        }
    },
    server: {
        port: 3000,
        proxy: {
            '/api': 'http://localhost:8443',
        },
    },
})