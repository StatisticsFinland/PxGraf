import React from 'react';
import { TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import useDebouncedCallback from 'hooks/useDebouncedCallback';

interface IRegexDimensionSelectionProps {
    pattern: string,
    onQueryChanged: (newPattern: string) => void
}

const DEBOUNCE_MS = 500;

const StyledTextField = styled(TextField)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
}));

// NOTE: this validates using JS RegExp syntax for immediate UI feedback, while the backend matches using
// .NET Regex. The two engines aren't fully identical, so a pattern accepted here could in rare cases be
// treated differently by the server.
const isValidPattern = (pattern: string): boolean => {
    try {
        new RegExp(pattern);
        return true;
    } catch {
        return false;
    }
}

export const RegexDimensionSelection: React.FC<IRegexDimensionSelectionProps> = ({ pattern, onQueryChanged }) => {
    const { t } = useTranslation();
    const [inputValue, setInputValue] = React.useState(pattern ?? '');
    const [previousPattern, setPreviousPattern] = React.useState(pattern);
    const debouncedOnQueryChanged = useDebouncedCallback(onQueryChanged, DEBOUNCE_MS);

    // Intentionally setting state during render (React's "adjusting state when a prop changes" pattern).
    // The guard below ensures this only runs once per actual prop change, avoiding render loops.
    if (pattern !== previousPattern) {
        setPreviousPattern(pattern);
        setInputValue(pattern ?? '');
    }

    const isValid = isValidPattern(inputValue);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newValue = event.target.value;
        setInputValue(newValue);
        // Invalid patterns are still submitted so that the resolved result reflects no matches.
        debouncedOnQueryChanged(newValue);
    }

    return (
        <StyledTextField label={t("variableSelect.regexPatternLabel")}
            value={inputValue}
            onChange={handleChange}
            error={!isValid}
            helperText={!isValid ? t('variableSelect.regexPatternError') : undefined}
            fullWidth
        />
    );
}

export default RegexDimensionSelection;
