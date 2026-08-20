import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { EDimensionType, IDimension } from 'types/cubeMeta';
import { ICubeQuery } from 'types/query';
import MetadataDialog from './MetadataDialog';

const dimension: IDimension = {
    code: 'dimension-1',
    name: { en: 'Dimension' },
    type: EDimensionType.Other,
    values: [
        {
            code: 'value-1',
            name: { en: 'Original value' },
            isVirtual: false,
        },
        {
            code: 'value-2',
            name: { en: 'Second value' },
            isVirtual: false,
        },
    ],
};

const initialCubeQuery: ICubeQuery = { variableQueries: {} };

const metadataDialog = (
    onApply = jest.fn(),
    onClose = jest.fn(),
    cubeQuery: ICubeQuery = initialCubeQuery,
) => (
    <UiLanguageContext.Provider value={{
        language: 'en',
        setLanguage: jest.fn(),
        languageTab: 'en',
        setLanguageTab: jest.fn(),
        availableUiLanguages: ['en'],
        uiContentLanguage: 'en',
        setUiContentLanguage: jest.fn(),
    }}>
        <MetadataDialog
            open
            dimensions={[dimension]}
            contentLanguages={['en']}
            initialLanguage="en"
            initialCubeQuery={cubeQuery}
            onApply={onApply}
            onClose={onClose}
        />
    </UiLanguageContext.Provider>
);

const renderDialog = (onApply = jest.fn(), onClose = jest.fn(), cubeQuery: ICubeQuery = initialCubeQuery) =>
    render(metadataDialog(onApply, onClose, cubeQuery));

describe('MetadataDialog', () => {
    it('associates both tab rows with the rendered value panel using unique IDs', () => {
        renderDialog();

        const dimensionTab = screen.getByRole('tab', { name: 'Dimension' });
        const languageTab = screen.getByRole('tab', { name: 'editor.contentLanguage: lang.local.en' });
        const panelId = dimensionTab.getAttribute('aria-controls');

        expect(dimensionTab.id).not.toBe('simple-tab-0');
        expect(languageTab.id).not.toBe('simple-tab-en');
        expect(languageTab).toHaveAttribute('aria-controls', panelId);
        expect(document.getElementById(panelId)).toHaveAttribute('role', 'tabpanel');
    });

    it('keeps edits local until Apply is pressed', () => {
        const onApply = jest.fn();
        const onClose = jest.fn();
        renderDialog(onApply, onClose);

        fireEvent.change(screen.getByDisplayValue('Original value'), { target: { value: 'Edited value' } });
        fireEvent.change(screen.getByDisplayValue('Second value'), { target: { value: 'Edited second value' } });
        expect(onApply).not.toHaveBeenCalled();

        fireEvent.click(screen.getByRole('button', { name: 'editMetadata.apply' }));

        expect(onApply).toHaveBeenCalledTimes(1);
        expect(onApply).toHaveBeenCalledWith({
            'dimension-1': {
                valueEdits: {
                    'value-1': {
                        nameEdit: { en: 'Edited value' },
                    },
                    'value-2': {
                        nameEdit: { en: 'Edited second value' },
                    },
                },
            }
        });
        expect(onClose).toHaveBeenCalledTimes(1);
    });

    it('discards edits when Cancel is pressed', () => {
        const onApply = jest.fn();
        const onClose = jest.fn();
        renderDialog(onApply, onClose);

        fireEvent.change(screen.getByDisplayValue('Original value'), { target: { value: 'Discarded value' } });
        fireEvent.click(screen.getByRole('button', { name: 'editMetadata.cancel' }));

        expect(onApply).not.toHaveBeenCalled();
        expect(onClose).toHaveBeenCalledTimes(1);
    });

    it('preserves its active draft when the global cube query changes', () => {
        const onApply = jest.fn();
        const onClose = jest.fn();
        const { rerender } = renderDialog(onApply, onClose);

        fireEvent.change(screen.getByDisplayValue('Original value'), { target: { value: 'Draft edit' } });
        rerender(metadataDialog(onApply, onClose, {
            chartHeaderEdit: { en: 'Updated heading' },
            variableQueries: {},
        }));
        fireEvent.click(screen.getByRole('button', { name: 'editMetadata.apply' }));

        expect(onApply).toHaveBeenCalledWith({
            'dimension-1': {
                valueEdits: {
                    'value-1': {
                        nameEdit: { en: 'Draft edit' },
                    },
                },
            },
        });
    });
});
