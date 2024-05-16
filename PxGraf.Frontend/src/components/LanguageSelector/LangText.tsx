import { Typography } from '@mui/material';
import React from 'react';
import styled from 'styled-components';

interface ILangTextProps {
    text: string;
    underline: boolean;
}

const UnderlinedTypography = styled(Typography)`
    text-decoration: underline;
`;

export const LangText: React.FC<ILangTextProps> = ({ text, underline }) => {
    return underline ? (<UnderlinedTypography>{text}</UnderlinedTypography>) : (<Typography>{text}</Typography>);
}

export default LangText;