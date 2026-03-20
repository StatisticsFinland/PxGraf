import React from 'react';
import Header from "./Header";
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';

jest.mock('envVars', () => ({
    PxGrafUrl: 'pxGrafUrl.fi/',
    PublicUrl: 'publicUrl.fi/',
    BasePath: ''
}));

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useLocation: () => ({
        foo: 'bar',
    }),
    useNavigate: () => ({
        navigate: jest.fn(),
    })
}));

jest.mock('../../contexts/navigationContext', () => ({
    ...jest.requireActual('../../contexts/navigationContext'),
    useNavigationContext: () => ({tablePath: null})
}))

describe('Header component', () => {
    it('should render correctly', async () => {
        const { asFragment } = render(<Header />);
        await waitFor(() => {
            expect(asFragment()).toMatchSnapshot();
        });
    })
});

describe('Assertion tests', () => {
    it('renders the page title', async () => {
        render(<Header />);
        await waitFor(() => {
            expect(screen.getByRole('heading', { level: 1 })).toBeInTheDocument();
        });
    });

    it('renders the database selector link', async () => {
        render(<Header />);
        await waitFor(() => {
            expect(screen.getByText('general.databaseSelectorLink')).toBeInTheDocument();
        });
    });

    it('renders the logo image with alt text', async () => {
        render(<Header />);
        await waitFor(() => {
            expect(screen.getByAltText('navbar.logoAlt')).toBeInTheDocument();
        });
    });

    it('renders the skip to content link', async () => {
        render(<Header />);
        await waitFor(() => {
            expect(screen.getByText('general.contentLink')).toBeInTheDocument();
        });
    });
});
