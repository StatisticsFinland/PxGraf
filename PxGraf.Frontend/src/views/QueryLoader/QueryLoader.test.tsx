import { render, waitFor } from "@testing-library/react";
import QueryLoader from "./QueryLoader";

import { HashRouter } from "react-router-dom";
import { fetchSavedQuery } from 'api/services/queries';

jest.mock('api/services/queries');
const mockNavigate = jest.fn();

jest.mock('react-router-dom', () => ({
    ...jest.requireActual('react-router-dom'),
    useNavigate: () => mockNavigate,
}));

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => {}),
            },
        };
    },
}));

describe('Rendering test', () => {

    it('renders correctly', () => {
        const { asFragment } = render(
            <HashRouter><QueryLoader /></HashRouter>
        );
        waitFor(() => {
            expect(asFragment()).toMatchSnapshot();
        });
    });
});

describe('Assertion tests', () => {
    it('displays error state', async () => {
        (fetchSavedQuery as jest.Mock).mockRejectedValueOnce(new Error('fetch error'));
        const { getByRole } = render(<HashRouter><QueryLoader /></HashRouter>);
        await waitFor(() => expect(getByRole('alert')).toBeInTheDocument());
    });

    it('displays loading state', () => {
        (fetchSavedQuery as jest.Mock).mockImplementation(() => new Promise(() => null));
        const { getByRole } = render(<HashRouter><QueryLoader /></HashRouter>);
        expect(getByRole('loading')).toBeInTheDocument();
    });

    it('navigates after successful fetch', async () => {
        (fetchSavedQuery as jest.Mock).mockResolvedValueOnce({
            query: {
                tableReference: {
                    name: 'foo.px',
                    hierarchy: ['bar', 'baz'],
                },
                variableQueries: {},
            },
            settings: {}
        });
        render(<HashRouter><QueryLoader /></HashRouter>);
        await waitFor(() => expect(mockNavigate.mock.calls[0][0]).toEqual("/editor/bar/baz/foo.px/"));
    });
});