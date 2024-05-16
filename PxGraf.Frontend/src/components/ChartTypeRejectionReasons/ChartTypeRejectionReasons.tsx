import { useTranslation } from 'react-i18next';
import React from 'react';
import { MultiLanguageString } from 'types/multiLanguageString';
import {
    Button, Dialog, DialogTitle,
    DialogContent, DialogActions
} from '@mui/material';
import QuestionMarkIcon from '@mui/icons-material/QuestionMark';
import UiLanguageContext from 'contexts/uiLanguageContext';

interface IChartTypeRejectionReasonsProps {
    rejectionReasons: { [key: string]: MultiLanguageString };
}

export const ChartTypeRejectionReasons: React.FC<IChartTypeRejectionReasonsProps> = ({ rejectionReasons }) => {
    const { t } = useTranslation();
    const { language } = React.useContext(UiLanguageContext);
    const [open, setOpen] = React.useState(false);

    const rejectionReasonElements: React.ReactNode[] = Object.keys(rejectionReasons).map(key => {
        return (
            <div key={key}>
                <span>{`${t('chartTypes.' + key)}:`}</span>
                <ul>
                    <li>
                        {rejectionReasons[key][language]}
                    </li>
                </ul>
            </div>
        );
    });
    if (!rejectionReasons) {
        return <></>;
    }
    return (
        <>
            <Button aria-haspopup={true} aria-controls='rejection-dialog' variant={'outlined'} startIcon={<QuestionMarkIcon />} onClick={() => setOpen(true)}>{t("rejectionDialog.buttonText")}</Button>
            <Dialog
                open={open}
                onClose={() => setOpen(false)}
                scroll='paper'
                aria-labelledby="scroll-dialog-title"
                aria-describedby="scroll-dialog-description"
            >
                <DialogTitle id="scroll-dialog-title">{t("rejectionDialog.titleText")}</DialogTitle>
                <DialogContent dividers={true} id="scroll-dialog-description">
                    {rejectionReasonElements}
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpen(false)}>{t("rejectionDialog.closeButton")}</Button>
                </DialogActions>
            </Dialog>
        </>
    );
}

export default ChartTypeRejectionReasons;