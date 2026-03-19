import React from 'react';
import Header from "./Header";
import { render, waitFor } from '@testing-library/react';

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
