import { Grid } from '@mui/material';
import React from 'react';
import { IDimension } from 'types/cubeMeta';
import { IVariableEditions, IVariableValueEditions } from 'types/query';
import { ContentVariableValueEditor } from './ContentVariableValueEditor';

interface IContentVariableEditorProps {
    variable: IDimension;
    language: string;
    variableEdits: IVariableEditions;
    onChange: (newEdit: IVariableEditions) => void;
}

export const ContentVariableEditor: React.FC<IContentVariableEditorProps> = ({ variable, language, variableEdits, onChange }) => {

    const handleChange = (newValueEdits: IVariableValueEditions, code: string) => {
        const newVariableEdit = {
            ...variableEdits,
            valueEdits: {
                ...variableEdits?.valueEdits,
                [code]: newValueEdits
            }
        };

        onChange(newVariableEdit);
    };

    return (
        <Grid container spacing={3}>
            {variable.Values.map((value) => {
                const valueEdits = variableEdits?.valueEdits[value.Code];

                return (
                    <Grid item
                        key={value.Code}
                        xs={12}
                        xl={variable.Values.length > 1 ? 6 : 12 /* Use two columns layout when screen is big enought and there is more than one value */}
                    >
                        <ContentVariableValueEditor
                            key={value.Code}
                            variableValue={value}
                            language={language}
                            valueEdits={valueEdits}
                            onChange={(newEdit) => handleChange(newEdit, value.Code)}
                        />
                    </Grid>
                );
            })}
        </Grid>
    );
}