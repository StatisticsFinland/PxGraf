import { Grid } from '@mui/material';
import React from 'react';
import { IContentDimensionValue, IDimension } from 'types/cubeMeta';
import { IDimensionValueEditions } from 'types/query';
import { ContentDimensionValueEditor } from './ContentDimensionValueEditor';
import { EditorContext } from '../../contexts/editorContext';

interface IContentDimensionEditorProps {
    dimension: IDimension;
    language: string;
}

export const ContentDimensionEditor: React.FC<IContentDimensionEditorProps> = ({ dimension, language }) => {
    const { cubeQuery, setCubeQuery } = React.useContext(EditorContext);
    const dimensionEdits = cubeQuery?.variableQueries[dimension.code];

    const handleChange = (newValueEdits: IDimensionValueEditions, code: string) => {
        const newDimensionEdit = {
            ...dimensionEdits,
            valueEdits: {
                ...dimensionEdits?.valueEdits,
                [code]: newValueEdits
            }
        };

        setCubeQuery({
            ...cubeQuery,
            variableQueries: {
                ...cubeQuery?.variableQueries,
                [dimension.code]: newDimensionEdit
            }
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