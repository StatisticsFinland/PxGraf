import React from 'react';
import Header from "./Header";
import { render, screen, waitFor } from '@testing-library/react';
import '@testing-library/jest-dom';
import { useNavigationContext } from '../../contexts/navigationContext';

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
    useNavigationContext: jest.fn(() => ({tablePath: null}))
}))

jest.mock('./BreadcrumbNav', () => ({
    __esModule: true,
    default: ({ tablePath }: { tablePath: string[] }) => <div data-testid="breadcrumb-nav">{tablePath.join('/')}</div>
}));

const mockUseNavigationContext = useNavigationContext as jest.Mock;

describe('Header component', () => {
    it('should render correctly', async () => {
        const { asFragment } = render(<Header />);
        await waitFor(() => {
            expect(asFragment()).toMatchSnapshot();
        });
    })
});

describe('Assertion tests', () => {
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

    it('renders breadcrumb nav when tablePath is set', async () => {
        mockUseNavigationContext.mockReturnValueOnce({ tablePath: ['db1', 'stat1', 'table1'] });
        render(<Header />);
        await waitFor(() => {
            expect(screen.getByTestId('breadcrumb-nav')).toBeInTheDocument();
            expect(screen.getByText('db1/stat1/table1')).toBeInTheDocument();
        });
    });
});
