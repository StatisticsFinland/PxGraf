import React from 'react';
import { fireEvent, render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { SaveDialog } from './SaveDialog';
import { EditorContext } from '../../contexts/editorContext';
import { VisualizationType } from '../../types/visualizationType';

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
const onSaveMock = jest.fn(() => { });
const setIsDraftMock = jest.fn();

describe('Rendering test', () => {
    it('renders correctly when closed', () => {
        const dom = render(<SaveDialog onSave={onSaveMock} />);
        expect(dom.baseElement).toMatchSnapshot();
    });

    it('renders correctly when open', () => {
        const dom = render(
            <EditorContext.Provider value={{
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: true,
                setSaveDialogOpen: onCloseMock,
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn(),
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: setIsDraftMock
            }}>
                <SaveDialog onSave={onSaveMock} />
            </EditorContext.Provider>
        );
        expect(dom.baseElement).toMatchSnapshot();
    });
});

describe('Assertion test', () => {
    beforeEach(() => {
        onCloseMock.mockClear();
        onSaveMock.mockClear();
        setIsDraftMock.mockClear();
    });

    it('invokes close function when cancel button is clicked', () => {
        render(
            <EditorContext.Provider value={{
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: true,
                setSaveDialogOpen: onCloseMock,
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn(),
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: setIsDraftMock
            }}>
                <SaveDialog onSave={onSaveMock} />
            </EditorContext.Provider>
        );
        fireEvent.click(screen.getByText('saveDialog.cancel'));
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });

    it('invokes save and close function when save button is clicked', () => {
        render(
            <EditorContext.Provider value={{
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: true,
                setSaveDialogOpen: onCloseMock,
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn(),
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: setIsDraftMock
            }}>
                <SaveDialog onSave={onSaveMock} />
            </EditorContext.Provider>
        );
        fireEvent.click(screen.getByText('saveDialog.save'));
        expect(onSaveMock).toHaveBeenCalledTimes(1);
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });

    it('invokes save and close function with draft as true when publish checkbox is unchecked', () => {
        render(
            <EditorContext.Provider value={{
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: true,
                setSaveDialogOpen: onCloseMock,
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn(),
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: setIsDraftMock
            }}>
                <SaveDialog onSave={onSaveMock} />
            </EditorContext.Provider>
        );

        // Checkbox is unchecked by default (saveAsPublished = false)
        fireEvent.click(screen.getByText('saveDialog.save'));
        expect(onSaveMock).toHaveBeenCalledWith(false, true); // static: false, draft: true
        expect(onSaveMock).toHaveBeenCalledTimes(1);
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });

    it('invokes save and close function with draft as false when publish checkbox is checked', () => {
        render(
            <EditorContext.Provider value={{
                cubeQuery: null,
                setCubeQuery: jest.fn(),
                query: {},
                setQuery: jest.fn(),
                saveDialogOpen: true,
                setSaveDialogOpen: onCloseMock,
                selectedVisualizationUserInput: VisualizationType.VerticalBarChart,
                setSelectedVisualizationUserInput: jest.fn(),
                visualizationSettingsUserInput: {},
                setVisualizationSettingsUserInput: jest.fn(),
                defaultSelectables: {},
                setDefaultSelectables: jest.fn(),
                loadedQueryId: '',
                setLoadedQueryId: jest.fn(),
                loadedQueryIsDraft: false,
                setLoadedQueryIsDraft: setIsDraftMock
            }}>
                <SaveDialog onSave={onSaveMock} />
            </EditorContext.Provider>
        );

        // Check the publish checkbox
        fireEvent.click(screen.getByText('saveDialog.publish'));
        fireEvent.click(screen.getByText('saveDialog.save'));
        expect(onSaveMock).toHaveBeenCalledWith(false, false); // static: false, draft: false
        expect(onSaveMock).toHaveBeenCalledTimes(1);
        expect(onCloseMock).toHaveBeenCalledTimes(1);
    });
});