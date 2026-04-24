import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import ErrorBoundary from './ErrorBoundary';

jest.mock('i18next', () => ({
    t: (key: string) => key,
}));

let consoleErrorSpy: jest.SpyInstance;

beforeEach(() => {
    consoleErrorSpy = jest.spyOn(console, 'error').mockImplementation(() => {});
});

afterEach(() => {
    consoleErrorSpy.mockRestore();
});

const ThrowingComponent = () => {
    throw new Error('Test error');
};

const SafeComponent = () => <div>Child content</div>;

describe('Rendering test', () => {
    it('renders children when no error occurs', () => {
        const { asFragment } = render(
            <ErrorBoundary>
                <SafeComponent />
            </ErrorBoundary>
        );
        expect(asFragment()).toMatchSnapshot();
    });

    it('renders error fallback when child throws', () => {
        const { asFragment } = render(
            <ErrorBoundary>
                <ThrowingComponent />
            </ErrorBoundary>
        );
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('displays child content when no error occurs', () => {
        render(
            <ErrorBoundary>
                <SafeComponent />
            </ErrorBoundary>
        );
        expect(screen.getByText('Child content')).toBeInTheDocument();
    });

    it('displays error alert when child throws', () => {
        render(
            <ErrorBoundary>
                <ThrowingComponent />
            </ErrorBoundary>
        );
        expect(screen.getByRole('alert')).toBeInTheDocument();
        expect(screen.getByText('error.boundaryTitle')).toBeInTheDocument();
        expect(screen.getByText('error.boundaryMessage')).toBeInTheDocument();
    });

    it('does not display child content when child throws', () => {
        render(
            <ErrorBoundary>
                <ThrowingComponent />
            </ErrorBoundary>
        );
        expect(screen.queryByText('Child content')).not.toBeInTheDocument();
    });

    it('recovers when try again button is clicked and child no longer throws', () => {
        let shouldThrow = true;
        const ConditionalThrower = () => {
            if (shouldThrow) throw new Error('Test error');
            return <div>Recovered content</div>;
        };

        render(
            <ErrorBoundary>
                <ConditionalThrower />
            </ErrorBoundary>
        );

        expect(screen.getByRole('alert')).toBeInTheDocument();

        shouldThrow = false;
        fireEvent.click(screen.getByText('error.boundaryRetry'));

        expect(screen.queryByRole('alert')).not.toBeInTheDocument();
        expect(screen.getByText('Recovered content')).toBeInTheDocument();
    });

    it('logs the error via console.error with the caught error', () => {
        render(
            <ErrorBoundary>
                <ThrowingComponent />
            </ErrorBoundary>
        );
        expect(consoleErrorSpy).toHaveBeenCalledWith(
            'ErrorBoundary caught an error:',
            expect.any(Error),
            expect.objectContaining({ componentStack: expect.any(String) })
        );
    });
});
