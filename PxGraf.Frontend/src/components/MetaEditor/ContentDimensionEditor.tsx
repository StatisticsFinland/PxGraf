import { Grid } from '@mui/material';
import React from 'react';
import { IDimension } from 'types/cubeMeta';
import { IDimensionEditions, IDimensionValueEditions } from 'types/query';
import { ContentDimensionValueEditor } from './ContentDimensionValueEditor';

interface IContentDimensionEditorProps {
    dimension: IDimension;
    language: string;
    dimensionEdits: IDimensionEditions;
    onChange: (newEdit: IDimensionEditions) => void;
}

export const ContentDimensionEditor: React.FC<IContentDimensionEditorProps> = ({ dimension, language, dimensionEdits, onChange }) => {

    const handleChange = (newValueEdits: IDimensionValueEditions, code: string) => {
        const newDimensionEdit = {
            ...dimensionEdits,
            valueEdits: {
                ...dimensionEdits?.valueEdits,
                [code]: newValueEdits
            }
        };

        onChange(newDimensionEdit);
    };

    return (
        <Grid container spacing={3}>
            {dimension.values.map((value) => {
                const valueEdits = dimensionEdits?.valueEdits[value.code];

                return (
                    <Grid item
                        key={value.code}
                        xs={12}
                        xl={dimension.values.length > 1 ? 6 : 12 /* Use two columns layout when screen is big enought and there is more than one value */}
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