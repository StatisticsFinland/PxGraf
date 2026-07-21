import { Typography } from '@mui/material';
import React from 'react';

interface ILangTextProps {
    text: string;
}

export const LangText: React.FC<ILangTextProps> = ({ text }) => {
    return <Typography>{text}</Typography>;
}

export default LangText;