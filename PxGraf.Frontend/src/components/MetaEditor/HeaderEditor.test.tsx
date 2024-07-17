import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import HeaderEditor from './HeaderEditor';

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

const mockDefaultResponse = {
    isLoading: false,
    isError: false,
    data: {
        'fi': 'foo'
    }
};

const mockDefaultResponseLoading = {
    isLoading: true,
    isError: false,
    data: {
        'fi': 'foo'
    }
};

const mockDefaultResponseError = {
    isLoading: false,
    isError: true,
    data: {
        'fi': 'foo'
    }
};

const mockEditValue = { 'fi': 'bar' }
const mockLang = 'fi';
const mockFunction = jest.fn();

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<HeaderEditor defaultHeaderResponse={mockDefaultResponse} editValue={mockEditValue} language={mockLang} onChange={mockFunction} style={{}} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly when loading', () => {
        const { asFragment } = render(<HeaderEditor defaultHeaderResponse={mockDefaultResponseLoading} editValue={mockEditValue} language={mockLang} onChange={mockFunction} style={{}} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly when receiving error', () => {
        const { asFragment } = render(<HeaderEditor defaultHeaderResponse={mockDefaultResponseError} editValue={mockEditValue} language={mockLang} onChange={mockFunction} style={{}} />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(<HeaderEditor defaultHeaderResponse={mockDefaultResponse} editValue={mockEditValue} language={mockLang} onChange={mockFunction} style={{}} />);
        fireEvent.change(screen.getByDisplayValue(mockEditValue['fi']), { target: { value: 'editValue2' } });
        expect(mockFunction).toBeCalledTimes(1);
    });
});
