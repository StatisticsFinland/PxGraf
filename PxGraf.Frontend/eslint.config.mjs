import js from '@eslint/js';
import tsPlugin from '@typescript-eslint/eslint-plugin';
import reactPlugin from 'eslint-plugin-react';
import reactHooksPlugin from 'eslint-plugin-react-hooks';
import reactRefreshPlugin from 'eslint-plugin-react-refresh';
import globals from 'globals';

export default [
    { ignores: ['node_modules/', 'build/'] },
    js.configs.recommended,
    ...tsPlugin.configs['flat/recommended'],
    {
        files: ['src/**/*.{ts,tsx}'],
        plugins: {
            react: reactPlugin,
            'react-hooks': reactHooksPlugin,
            'react-refresh': reactRefreshPlugin,
        },
        languageOptions: {
            globals: {
                ...globals.browser,
                ...globals.es2021,
            },
        },
        rules: {
            ...reactPlugin.configs.flat.recommended.rules,
            ...reactPlugin.configs.flat['jsx-runtime'].rules,
            ...reactHooksPlugin.configs.flat.recommended.rules,
            'react-refresh/only-export-components': ['warn', { allowConstantExport: true }],
        },
        settings: {
            react: { version: 'detect' },
        },
    },
    // Context files legitimately export both context objects and Provider components
    {
        files: ['src/contexts/*.{ts,tsx}'],
        rules: {
            'react-refresh/only-export-components': 'off',
        },
    },
    // These files export utility helpers alongside components by design
    {
        files: ['src/Router.tsx', 'src/components/Preview/Preview.tsx'],
        rules: {
            'react-refresh/only-export-components': 'off',
        },
    },
];
