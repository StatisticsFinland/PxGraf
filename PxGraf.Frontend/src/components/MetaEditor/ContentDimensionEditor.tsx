import { Grid } from '@mui/material';
import React from 'react';
import { IContentDimensionValue, IDimension } from 'types/cubeMeta';
import { ICubeQuery, IDimensionValueEditions } from 'types/query';
import { ContentDimensionValueEditor } from './ContentDimensionValueEditor';
import { QueryContext } from '../../contexts/queryContext';

interface IContentDimensionEditorProps {
    dimension: IDimension;
    language: string;
}

export const ContentDimensionEditor: React.FC<IContentDimensionEditorProps> = ({ dimension, language }) => {
    const { cubeQuery, setCubeQuery } = React.useContext(QueryContext);
    const dimensionEdits = cubeQuery?.variableQueries[dimension.code];

    const handleChange = (update: React.SetStateAction<IDimensionValueEditions>, code: string) => {
        setCubeQuery((currentCubeQuery: ICubeQuery) => {
            const currentDimensionEdits = currentCubeQuery.variableQueries[dimension.code];
            const currentValueEdits = currentDimensionEdits?.valueEdits?.[code] ?? {};
            const nextValueEdits = typeof update === 'function' ? update(currentValueEdits) : update;
            return {
                ...currentCubeQuery,
                variableQueries: {
                    ...currentCubeQuery.variableQueries,
                    [dimension.code]: {
                        ...currentDimensionEdits,
                        valueEdits: {
                            ...currentDimensionEdits?.valueEdits,
                            [code]: nextValueEdits
                        }
                    }
                }
            };
        });
    };

    return (
        <Grid container spacing={3}>
            {dimension.values.map((value: IContentDimensionValue) => {
                const valueEdits = dimensionEdits?.valueEdits[value.code];

                return (
                    <Grid 
                        key={value.code}
                        size={{ xs: 12, xl: dimension.values.length > 1 ? 6 : 12 }}
                        columns={dimension.values.length > 1 ? 1 : 2}
                    >
                        <ContentDimensionValueEditor
                            key={value.code}
                            dimensionValue={value}
                            language={language}
                            valueEdits={valueEdits}
                            onChange={(newEdit) => handleChange(newEdit, value.code)}
                        />
                    </Grid>
                );
            })}
        </Grid>
    );
}