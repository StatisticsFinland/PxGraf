import React, { useId } from 'react';
import { useTranslation } from 'react-i18next';
import { FormControl, InputLabel, OutlinedInput, InputAdornment, Alert } from '@mui/material';
import styled from 'styled-components';
import RevertButton from './RevertButton';

interface IEditorFieldProps {
    label: string;
    defaultValue: string;
    editValue: string;
    onChange: (value?: string) => void;
    style?: { [key: string]: string | number };
    maxLength?: number;
}

const StyledOutlinedInput = styled(OutlinedInput)<{ $isEdited: boolean }>(({ $isEdited }) => ({
    backgroundColor: $isEdited ? 'var(--editorfield-background-edited)' : 'var(--editorfield-background)',
    '& .MuiOutlinedInput-notchedOutline': {
        borderColor: $isEdited ? 'var(--editorfield-outline-edited)' : 'var(--editorfield-outline)',
    },
    '& input': {
        fontWeight: $isEdited ? 'bold' : 'normal',
    }
}));

export const EditorField: React.FC<IEditorFieldProps> = ({ label, defaultValue, editValue, onChange, maxLength, style = {} }) => {
    const { t } = useTranslation();
    const inputId = useId();
    const ALERT_THRESHOLD = 0.556;
    const [localValue, setLocalValue] = React.useState(editValue ?? defaultValue);
    const isEdited: boolean = editValue != null;
    const showAlert = maxLength && (localValue.length / maxLength) > ALERT_THRESHOLD;

    React.useEffect(() => {
        const value = editValue ?? defaultValue;
        setLocalValue(value);
    }, [defaultValue, editValue]);

    return (
        <FormControl variant="outlined" style={style}>
            <InputLabel htmlFor={inputId}>{isEdited ? <b>{label + '*'}</b> : label}</InputLabel>
            <StyledOutlinedInput
                id={inputId}
                type='text'
                value={localValue}
                onChange={evt => {
                    const parsedValue = evt.target.value.substring(0, maxLength || evt.target.value.length);
                    setLocalValue(parsedValue);
                    onChange(parsedValue);
                }}
                endAdornment={
                    isEdited && (
                        <InputAdornment position="end">
                            <RevertButton onClick={() => {
                                setLocalValue(defaultValue);
                                onChange();
                            }}/>
                        </InputAdornment>
                    )
                }
                label={label}
                $isEdited={isEdited}
            />
            <div aria-live='polite'>
            {
                showAlert &&
                    <Alert severity={(localValue.length / maxLength) < 1 ? 'warning' : 'error'}>
                        {`${t('titleWarning.maxLengthText')} ${maxLength} ${t('titleWarning.charactersText')}. ${t('titleWarning.usedLengthText')} ${localValue.length} ${t('titleWarning.charactersText')}.`}
                </Alert>
            }
            </div>
        </FormControl>
    );
}

export default EditorField;