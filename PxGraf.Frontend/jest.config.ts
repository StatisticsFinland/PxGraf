import type { Config } from '@jest/types';

const config: Config.InitialOptions = {
    preset: 'ts-jest/presets/js-with-ts',
    testEnvironment: 'jest-environment-jsdom',
    coverageReporters: ['clover', 'json', 'lcov', 'text', 'cobertura'],
    modulePaths: ['<rootDir>/src'],
    moduleNameMapper: {
        '\\.png$': '<rootDir>/__mocks__/fileMock.ts',
    },
    setupFilesAfterEnv: ['<rootDir>/jest.setup.ts'],
};

export default config;