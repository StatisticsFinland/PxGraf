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
    const ALERT_TRESHOLD = 0.556;
    const [localValue, setLocalValue] = React.useState(defaultValue);

    const value: string = editValue ?? defaultValue;
    const isEdited: boolean = editValue != null;
    const showAlert = maxLength && (value.length / maxLength) > ALERT_TRESHOLD;

    React.useEffect(() => {
        setLocalValue(defaultValue);
    }, [defaultValue]);

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
                }}
                onBlur={() => onChange(localValue)}
                endAdornment={
                    isEdited && (
                        <InputAdornment position="end">
                            <RevertButton onClick={() => {
                                setLocalValue(defaultValue);
                                onChange(defaultValue);
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
                <Alert severity={(value.length / maxLength) < 1 ? 'warning' : 'error'}>
                    {`${t('titleWarning.maxLengthText')} ${maxLength} ${t('titleWarning.charactersText')}. ${t('titleWarning.usedLengthText')} ${value.length} ${t('titleWarning.charactersText')}.`}
                </Alert>
            }
            </div>
        </FormControl>
    );
}

export default EditorField;