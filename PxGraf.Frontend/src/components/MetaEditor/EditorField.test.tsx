import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import EditorField from './Editorfield';

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

const mockFunction = jest.fn();
const mockLabel = 'label';
const mockEditValue = 'editValue';
const mockDefaultValue = 'defaultValue';

describe('Rendering test', () => {
    it('renders correctly', () => {
        const { asFragment } = render(<EditorField defaultValue={mockDefaultValue} editValue={mockEditValue} label={mockLabel} onChange={mockFunction} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly if limit specified but values are short', () => {
        const { asFragment } = render(<EditorField maxLength={11} defaultValue={'foo'} editValue={'foo'} label={mockLabel} onChange={mockFunction} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly if limit reached', () => {
        const { asFragment } = render(<EditorField maxLength={11} defaultValue={'foobarbaz12'} editValue={'foobarbaz12'} label={mockLabel} onChange={mockFunction} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly if limit close', () => {
        const { asFragment } = render(<EditorField maxLength={11} defaultValue={'foobarbaz1'} editValue={'foobarbaz1'} label={mockLabel} onChange={mockFunction} />);
        expect(asFragment()).toMatchSnapshot();
    });
    it('renders correctly if the value hsa not been edited', () => {
        const { asFragment } = render(<EditorField defaultValue={mockDefaultValue} editValue={null} label={mockLabel} onChange={mockFunction} />);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion tests', () => {
    it('Change event should fire when value has changed', () => {
        render(<EditorField defaultValue={mockDefaultValue} editValue={mockEditValue} label={mockLabel} onChange={mockFunction} />);
        fireEvent.change(screen.getByDisplayValue(mockEditValue), { target: { value: 'editValue2' } });
        expect(mockFunction).toHaveBeenCalledTimes(1);
    });
});
