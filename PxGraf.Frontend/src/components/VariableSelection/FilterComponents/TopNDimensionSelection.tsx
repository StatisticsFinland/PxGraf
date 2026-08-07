import React from 'react';
import { TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { styled } from '@mui/material/styles';
import useDebouncedCallback from 'hooks/useDebouncedCallback';

interface ITopNDimensionSelectionProps {
    numberOfItems: number,
    onNumberChanged: (newValue: number) => void
}

const DEBOUNCE_MS = 500;

const StyledTextField = styled(TextField)(({ theme }) => ({
    backgroundColor: theme.palette.background.paper,
}));

export const TopNDimensionSelection: React.FC<ITopNDimensionSelectionProps> = ({ numberOfItems, onNumberChanged }) => {
    const { t } = useTranslation();
    const [inputValue, setInputValue] = React.useState(numberOfItems?.toString() ?? '');
    const [previousNumberOfItems, setPreviousNumberOfItems] = React.useState(numberOfItems);
    const debouncedOnNumberChanged = useDebouncedCallback(onNumberChanged, DEBOUNCE_MS);

    // Intentionally setting state during render (React's "adjusting state when a prop changes" pattern).
    // The guard below ensures this only runs once per actual prop change, avoiding render loops.
    if (numberOfItems !== previousNumberOfItems) {
        setPreviousNumberOfItems(numberOfItems);
        setInputValue(numberOfItems?.toString() ?? '');
    }

    const isValid = /^\d+$/.test(inputValue) && Number.parseInt(inputValue, 10) >= 1;

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const newValue = event.target.value;
        setInputValue(newValue);

        if (/^\d+$/.test(newValue) && Number.parseInt(newValue, 10) >= 1) {
            debouncedOnNumberChanged(Number.parseInt(newValue, 10));
        } else {
            debouncedOnNumberChanged.cancel();
        }
    }

    return (
        <StyledTextField label={t("variableSelect.latestValuesCountLabel")}
            value={inputValue}
            onChange={handleChange}
            error={!isValid}
            helperText={!isValid ? t('variableSelect.latestValuesCountError') : undefined}
            fullWidth
            slotProps={{ htmlInput: { inputMode: 'numeric', pattern: '[0-9]*', min: 1 } }}
        />
    );
}

export default TopNDimensionSelection;