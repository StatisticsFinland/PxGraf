import { Popper } from "@mui/material";
import { styled } from '@mui/material/styles';
import React from 'react';
import InfoIcon from '@mui/icons-material/Info';
import { useTranslation } from "react-i18next";
interface IInfoBubbleProps {
    info: React.ReactNode;
    ariaLabel: string;
    placement?: 'auto' | 'auto-end' | 'auto-start' | 'top' | 'top-end' | 'top-start' | 'left' | 'left-end' | 'left-start' | 'right' | 'right-end' | 'right-start' | 'bottom' | 'bottom-end' | 'bottom-start';
    id?: string;
}

const InfoButton = styled('button')({ background: 'transparent', border: 'none' });

const PopperInfo = styled('div')(({ theme }) => ({
    backgroundColor: theme.palette.info.main,
    color: theme.palette.info.contrastText,
    border: 'none',
    padding: 16,
    boxShadow: theme.shadows[3],
    borderRadius: theme.shape.borderRadius,
    maxWidth: 300,
}));

const StyledPopper = styled(Popper)({ zIndex: 999 });

export const InfoBubble: React.FC<IInfoBubbleProps> = ({ info, ariaLabel, placement = 'auto', id = null }) => {
    const [open, setOpen] = React.useState(false);
    const [anchorElement, setAnchorElement] = React.useState<HTMLButtonElement | null>(null);
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
                ref={setAnchorElement}
            >
                <InfoIcon color={'info'} />
            </InfoButton>
            <StyledPopper role="alert" keepMounted popperOptions={{ placement: placement }} open={open} anchorEl={anchorElement} onMouseEnter={() => setOpen(true)} onMouseLeave={() => setOpen(false)} >
                <PopperInfo>
                    {info}
                </PopperInfo>
            </StyledPopper>
        </>
    );
}

export default InfoBubble;