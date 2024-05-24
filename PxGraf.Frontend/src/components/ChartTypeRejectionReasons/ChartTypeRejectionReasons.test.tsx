import { fireEvent, render, screen } from '@testing-library/react';
import UiLanguageContext from 'contexts/uiLanguageContext';
import React from 'react';
import ChartTypeRejectionReasons from './ChartTypeRejectionReasons';

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

const setLanguage = jest.fn();
const language = 'fi';
const setLanguageTab = jest.fn();
const languageTab = 'fi';
const availableUiLanguages = ['fi', 'en', 'sv'];
const uiContentLanguage = 'fi';
const setUiContentLanguage = jest.fn();

const mockRejectionReasons = {
    "LineChart": {
        'fi': 'huono kuvio'
    },
    "PieChart": {
        'fi': 'vielä huonompi kuvio'
    }
}

describe('Rendering test', () => {
    it('renders correctly with values within range', () => {
        const { asFragment } = render(<UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}><ChartTypeRejectionReasons rejectionReasons={mockRejectionReasons} /></UiLanguageContext.Provider>);
        expect(asFragment()).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    it('should open the dialog when the button is clicked', () => {
        render(<UiLanguageContext.Provider value={{ language, setLanguage, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }}><ChartTypeRejectionReasons rejectionReasons={mockRejectionReasons} /></UiLanguageContext.Provider>);
        fireEvent.click(screen.getByText('rejectionDialog.buttonText'));
        expect(screen.getByText(mockRejectionReasons['LineChart']['fi'])).toBeInTheDocument();
        expect(screen.getByText(mockRejectionReasons['PieChart']['fi'])).toBeInTheDocument();
    });
});