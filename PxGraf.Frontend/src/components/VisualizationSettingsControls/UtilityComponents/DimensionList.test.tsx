import React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import DimensionList from './DimensionList';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { IDimension, EDimensionType } from 'types/cubeMeta';
import '@testing-library/jest-dom';

const mockDimensions: IDimension[] = [
    {
        code: 'dim1',
        name: { fi: 'Alue', en: 'Region' },
        type: EDimensionType.Other,
        values: [
            { code: 'v1', name: { fi: 'Helsinki' }, isVirtual: false },
            { code: 'v2', name: { fi: 'Tampere' }, isVirtual: false },
        ],
    },
    {
        code: 'dim2',
        name: { fi: 'Vuosi', en: 'Year' },
        type: EDimensionType.Time,
        values: [
            { code: 'y1', name: { fi: '2020' }, isVirtual: false },
        ],
    },
];

const uiContextValue = {
    language: 'fi',
    setLanguage: jest.fn(),
    languageTab: 'fi',
    setLanguageTab: jest.fn(),
    availableUiLanguages: ['fi', 'en'],
    uiContentLanguage: 'fi',
    setUiContentLanguage: jest.fn(),
};

const renderWithContext = (props: {
    title: string;
    dimensions: IDimension[];
    selectedDimensionCode: string;
    selectedChangedHandler: (code: string) => void;
}) => {
    return render(
        <UiLanguageContext.Provider value={uiContextValue}>
            <DimensionList {...props} />
        </UiLanguageContext.Provider>
    );
};

describe('DimensionList', () => {
    it('renders the title as a subheader', () => {
        renderWithContext({
            title: 'Muuttujat',
            dimensions: mockDimensions,
            selectedDimensionCode: 'dim1',
            selectedChangedHandler: jest.fn(),
        });
        expect(screen.getByText('Muuttujat')).toBeInTheDocument();
    });

    it('renders dimension names with value counts', () => {
        renderWithContext({
            title: 'Title',
            dimensions: mockDimensions,
            selectedDimensionCode: 'dim1',
            selectedChangedHandler: jest.fn(),
        });
        expect(screen.getByText('Alue 2')).toBeInTheDocument();
        expect(screen.getByText('Vuosi 1')).toBeInTheDocument();
    });

    it('calls selectedChangedHandler when a dimension is clicked', async () => {
        const handler = jest.fn();
        renderWithContext({
            title: 'Title',
            dimensions: mockDimensions,
            selectedDimensionCode: 'dim1',
            selectedChangedHandler: handler,
        });
        await userEvent.click(screen.getByText('Vuosi 1'));
        expect(handler).toHaveBeenCalledWith('dim2');
    });

    it('renders empty list when no dimensions provided', () => {
        renderWithContext({
            title: 'Empty',
            dimensions: [],
            selectedDimensionCode: '',
            selectedChangedHandler: jest.fn(),
        });
        expect(screen.getByText('Empty')).toBeInTheDocument();
        expect(screen.queryByRole('button')).toBeNull();
    });

    it('falls back to dimension code when name is missing for the content language', () => {
        const dims: IDimension[] = [
            {
                code: 'noname',
                name: { en: 'English only' },
                type: EDimensionType.Other,
                values: [{ code: 'v1', name: { fi: 'val' }, isVirtual: false }],
            },
        ];
        renderWithContext({
            title: 'Fallback',
            dimensions: dims,
            selectedDimensionCode: '',
            selectedChangedHandler: jest.fn(),
        });
        // fi name is missing, should fall back to code
        expect(screen.getByText('noname 1')).toBeInTheDocument();
    });
});
