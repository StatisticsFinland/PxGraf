import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        base: env.VITE_BASE_PATH || '/',
        plugins: [react()],
        resolve: {
            alias: {
                'Router': '/src/Router.tsx',
                'envVars': '/src/envVars.ts',
                'contexts': '/src/contexts',
                'types': '/src/types',
                'styles': '/src/styles',
                'views': '/src/views',
                'components': '/src/components',
                'hooks': '/src/hooks',
                'api': '/src/api',
                'utils': '/src/utils',
                'images': '/src/images'
            }
        },
        server: {
            port: 3000,
            proxy: {
                '/api': env.VITE_PXGRAF_URL,
            },
        },
        build: {
            outDir: './build',
        }
    }
})
