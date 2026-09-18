import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'
import packageJson from './package.json'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        base: env.VITE_BASE_PATH || '/',
        define: {
            'import.meta.env.VITE_APP_VERSION': JSON.stringify(packageJson.version),
        },
        plugins: [react()],
        resolve: {
            alias: {
                'Router': '/src/Router.tsx',
                'routes/urls': '/src/routes/urls.ts',
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
                '/api': env.VITE_PXGRAF_PROXY_TARGET || 'http://localhost:5000',
            },
        },
        build: {
            outDir: './build',
        }
    }
})
