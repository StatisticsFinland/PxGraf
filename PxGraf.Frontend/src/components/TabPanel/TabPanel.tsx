import { Box } from '@mui/material';
import React from 'react';
import styled from 'styled-components';

interface ITabPanelProps {
    value: string | number;
    selectedValue: string | number;
    children: React.ReactNode;
    idPrefix?: string;
}

const Wrapper = styled(Box)`
  padding: 24px;
`;

export const TabPanel: React.FC<ITabPanelProps> = (props) => {
    const { children, value, selectedValue, idPrefix, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={selectedValue !== value}
            id={idPrefix ? `${idPrefix}-panel-${value}` : `simple-tabpanel-${value}`}
            aria-labelledby={idPrefix ? `${idPrefix}-${value}` : `simple-tab-${value}`}
            {...other}
        >
            {selectedValue === value && (
                <Wrapper>
                    {children}
                </Wrapper>
            )}
        </div>
    );
}

export default TabPanel;