import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { SaveDialog } from './SaveDialog';

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

const onCloseMock = jest.fn();
const onSaveMock = jest.fn((bool: boolean) => { });

describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const dom = render(<SaveDialog open={false} onClose={onCloseMock} onSave={onSaveMock} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(<SaveDialog open={true} onClose={onCloseMock} onSave={onSaveMock} />);
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    beforeEach(() => {
        onCloseMock.mockClear();
        onSaveMock.mockClear();
    });

    it('invokes close function when cancel button is clicked', () => {
        render(<SaveDialog open={true} onClose={onCloseMock} onSave={onSaveMock} />);
        fireEvent.click(screen.getByText('saveDialog.cancel'));
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });

    it('invokes save and close function when save button is clicked', () => {
        render(<SaveDialog open={true} onClose={onCloseMock} onSave={onSaveMock} />);
        fireEvent.click(screen.getByText('saveDialog.save'));
        expect(onSaveMock).toHaveBeenCalledTimes(1);
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });
});