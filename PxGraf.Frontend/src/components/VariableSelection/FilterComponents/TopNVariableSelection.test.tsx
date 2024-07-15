import { fireEvent, render, screen } from "@testing-library/react";

import TopNVariableSelection from "./TopNVariableSelection";

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

const mockChangeFunction = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<TopNVariableSelection
            numberOfItems={2}
            onNumberChanged={(_newValue: number) => { }}
        ></TopNVariableSelection>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('should call onchange with the correct value when value is convertable to number', () => {
        render(<TopNVariableSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNVariableSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.topFilter'), { target: { value: '5' } });
        expect(mockChangeFunction).toBeCalledWith(5);
    });

    it('should call onchange with 0 when value is not convertable to number', () => {
        render(<TopNVariableSelection
            numberOfItems={2}
            onNumberChanged={mockChangeFunction}
        ></TopNVariableSelection>);
        fireEvent.change(screen.getByLabelText('variableSelect.topFilter'), { target: { value: 'eivoikaantaa' } });
        expect(mockChangeFunction).toBeCalledWith(0);
    });
});