import { Popper } from "@mui/material";
import React from "react";
import styled from "styled-components";
import InfoIcon from '@mui/icons-material/Info';
import { useTranslation } from "react-i18next";
interface IInfoBubbleProps {
    info: React.ReactNode;
    ariaLabel: string;
    placement?: 'auto' | 'auto-end' | 'auto-start' | 'top' | 'top-end' | 'top-start' | 'left' | 'left-end' | 'left-start' | 'right' | 'right-end' | 'right-start' | 'bottom' | 'bottom-end' | 'bottom-start';
    id?: string;
}

const InfoButton = styled.button`
    background: transparent;
    border: none;
`;

const PopperInfo = styled.div`
    background-color: #0073b0;
    color: #fff;
    border: none;
    padding: 16px;
    box-shadow: 3px 3px 10px #737373;
    border-radius: 5px;
    max-width: 300px;
`;

const StyledPopper = styled(Popper)`
    z-index: 999;
`;

export const InfoBubble: React.FC<IInfoBubbleProps> = ({ info, ariaLabel, placement = 'auto', id = null }) => {
    const [open, setOpen] = React.useState(false);
    const anchorElement = React.useRef(null);
    const { t } = useTranslation();

    // Event listener for pressing the escape key to close the info bubble
    React.useEffect(() => {
        const handleEscape = (event: KeyboardEvent) => {
            if (event.key === 'Escape') {
                setOpen(false);
            }
        };

        if (open) {
            window.addEventListener('keydown', handleEscape);
        } else {
            window.removeEventListener('keydown', handleEscape);
        }

        return () => {
            window.removeEventListener('keydown', handleEscape);
        };
    }, [open]);

    return (
        <>
            <InfoButton
                id={id}
                aria-label={`${open ? t('tooltip.close') : t('tooltip.open')}: ${ariaLabel}`}
                onMouseEnter={() => setOpen(true)}
                onMouseLeave={() => setOpen(false)}
                onClick={() => setOpen(!open)}
                ref={anchorElement}
            >
                <InfoIcon color={'info'} />
            </InfoButton>
            <StyledPopper role="alert" keepMounted popperOptions={{ placement: placement }} open={open} anchorEl={anchorElement.current} onMouseEnter={() => setOpen(true)} onMouseLeave={() => setOpen(false)} >
                <PopperInfo>
                    {info}
                </PopperInfo>
            </StyledPopper>
        </>
    );
}

export default InfoBubble;