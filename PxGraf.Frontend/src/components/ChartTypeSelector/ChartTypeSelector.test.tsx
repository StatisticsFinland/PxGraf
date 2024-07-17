import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import { ChartTypeSelector } from './ChartTypeSelector';
import '@testing-library/jest-dom';

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

const onTypeSelectedMock = jest.fn((value: string) => value);

const mockTypes = ['foo', 'bar', 'baz'];

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<ChartTypeSelector onTypeSelected={onTypeSelectedMock} possibleTypes={mockTypes} selectedType={'foo'} />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('should invoke handler if clicked', () => {
        render(<ChartTypeSelector onTypeSelected={onTypeSelectedMock} possibleTypes={mockTypes} selectedType={'foo'} />);
        fireEvent.click(screen.getByText('chartTypes.bar'));
        expect(onTypeSelectedMock).toBeCalledTimes(1);
    });
});