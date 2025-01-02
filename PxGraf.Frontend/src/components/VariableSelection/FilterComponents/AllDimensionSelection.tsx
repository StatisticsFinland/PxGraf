import React from 'react';

import { Box, Typography } from '@mui/material';
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';

const ComponentWrapper = styled(Box)`
  align-self: stretch;
  display: flex;
  align-items: center;
`;

export const AllDimensionSelection: React.FC = () => {
    const { t } = useTranslation();
    return (<ComponentWrapper><Typography>{t("variableSelect.allFilter")}</Typography></ComponentWrapper>);
}

export default AllDimensionSelection;