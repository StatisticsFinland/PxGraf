import React, { useId } from 'react';
import { useTranslation } from 'react-i18next';
import { FormControl, InputLabel, OutlinedInput, InputAdornment, Alert } from '@mui/material';
import { styled } from '@mui/material/styles';
import RevertButton from './RevertButton';

interface IEditorFieldProps {
    label: string;
    defaultValue: string;
    editValue: string;
    onChange: (value?: string) => void;
    style?: { [key: string]: string | number };
    maxLength?: number;
}

const StyledOutlinedInput = styled(OutlinedInput, {
    shouldForwardProp: prop => prop !== 'isEdited',
})<{ isEdited: boolean }>(({ isEdited, theme }) => ({
    backgroundColor: isEdited ? theme.palette.warning.light : theme.palette.background.paper,
    transition: 'background-color 120ms ease, box-shadow 120ms ease',
    '& .MuiOutlinedInput-notchedOutline': {
        borderColor: isEdited ? theme.palette.warning.dark : theme.palette.text.disabled,
    },
    '&:hover .MuiOutlinedInput-notchedOutline': {
        borderColor: isEdited ? theme.palette.warning.dark : theme.palette.text.secondary,
    },
    '&.Mui-focused': {
        boxShadow: isEdited ? `0 0 0 1px ${theme.palette.warning.dark}` : 'none',
    },
    '& input': {
        fontWeight: isEdited ? theme.typography.fontWeightBold : theme.typography.fontWeightRegular,
    }
}));

export const EditorField: React.FC<IEditorFieldProps> = ({ label, defaultValue, editValue, onChange, maxLength, style = {} }) => {
    const { t } = useTranslation();
    const inputId = useId();
    const ALERT_THRESHOLD = 0.556;
    const [localValue, setLocalValue] = React.useState(editValue ?? defaultValue);
    const [isEdited, setIsEdited] = React.useState(editValue != null);
    const showAlert = maxLength && (localValue.length / maxLength) > ALERT_THRESHOLD;

    React.useEffect(() => {
        const value = editValue ?? defaultValue;
        // eslint-disable-next-line react-hooks/set-state-in-effect -- intentional: syncs local state when props change
        setLocalValue(value);
        setIsEdited(editValue != null);
    }, [defaultValue, editValue]);

    return (
        <FormControl variant="outlined" size="small" style={style}>
            <InputLabel htmlFor={inputId}>{isEdited ? <b>{label + '*'}</b> : label}</InputLabel>
            <StyledOutlinedInput
                id={inputId}
                size="small"
                type='text'
                value={localValue}
                onChange={evt => {
                    const parsedValue = evt.target.value.substring(0, maxLength || evt.target.value.length);
                    setLocalValue(parsedValue);
                    setIsEdited(true);
                    onChange(parsedValue);
                }}
                endAdornment={
                    isEdited && (
                        <InputAdornment position="end">
                            <RevertButton onClick={() => {
                                setLocalValue(defaultValue);
                                setIsEdited(false);
                                onChange();
                            }}/>
                        </InputAdornment>
                    )
                }
                label={label}
                isEdited={isEdited}
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