/* istanbul ignore file */

import * as React from 'react';
import { QueryProvider } from './queryContext';
import { VisualizationProvider } from './visualizationContext';
import { SaveProvider } from './saveContext';

/**
 * Composite provider that wraps all editor sub-contexts.
 * Components should consume the specific context they need:
 * - QueryContext for cubeQuery / query
 * - VisualizationContext for visualization type / settings / defaultSelectables
 * - SaveContext for save dialog / loaded query / publication state
 */
export const EditorProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => (
    <QueryProvider>
        <VisualizationProvider>
            <SaveProvider>
                {children}
            </SaveProvider>
        </VisualizationProvider>
    </QueryProvider>
);