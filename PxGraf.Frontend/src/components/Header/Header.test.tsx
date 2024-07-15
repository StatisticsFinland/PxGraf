import { render } from "@testing-library/react";
import Header from "./Header";

jest.mock('react-i18next', () => ({
    ...jest.requireActual('react-i18next'),
    useTranslation: () => {
        return {
            t: (str: string) => str,
            i18n: {
                changeLanguage: () => new Promise(() => null),
            },
        };
    },
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
    it('should render correctly', () => {
        const { asFragment } = render(<Header />);
        expect(asFragment()).toMatchSnapshot();
    })
});
