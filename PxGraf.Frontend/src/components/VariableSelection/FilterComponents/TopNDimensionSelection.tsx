import React from 'react';
import { TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import styled from 'styled-components';

interface ITopNDimensionSelectionProps {
    numberOfItems: number,
    onNumberChanged: (newValue: number) => void
}

const StyledTextField = styled(TextField)`
    background-color: #fff;
`;

export const TopNDimensionSelection: React.FC<ITopNDimensionSelectionProps> = ({ numberOfItems, onNumberChanged }) => {
    const { t } = useTranslation();

    const handleChange = (event) => {
        const regExp = /^\d+$/;
        if (regExp.test(event.target.value)) {
            onNumberChanged(parseInt(event.target.value));
        }
        else {
            onNumberChanged(0);
        }
    }

    return (
        <StyledTextField label={t("variableSelect.topFilter")}
            defaultValue={numberOfItems?.toString() ?? ""}
            onChange={handleChange}
            fullWidth
            inputProps={{ inputMode: 'numeric', pattern: '[0-9]*' }}
        />
    );
}

export default TopNDimensionSelection;