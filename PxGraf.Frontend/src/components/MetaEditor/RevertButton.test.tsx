import { render } from '@testing-library/react';
import '@testing-library/jest-dom';
import RevertButton from "./RevertButton";

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<RevertButton onClick={() => undefined} />);
        expect(asFragment()).toMatchSnapshot();
    });
});