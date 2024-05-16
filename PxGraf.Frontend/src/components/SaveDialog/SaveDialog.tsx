import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';

import {
    FormControlLabel, FormControl, Button, Dialog, DialogTitle,
    DialogContent, DialogActions, FormLabel, RadioGroup, Radio,
} from '@mui/material';

import SaveIcon from '@mui/icons-material/Save';

interface ISaveDialogProps {
    open: boolean;
    onClose: () => void;
    onSave: (archive: boolean) => void;
}

export const SaveDialog: React.FC<ISaveDialogProps> = ({ open, onClose, onSave }) => {
    const { t } = useTranslation();
    const [selected, setSelected] = useState("dynamic");

    const saveAndClose = () => {
        onSave(selected === "static");
        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={onClose}
            scroll='paper'
            aria-labelledby="scroll-dialog-title"
            aria-describedby="scroll-dialog-description"
        >
            <DialogTitle id="scroll-dialog-title">{t("saveDialog.saveQuery")}</DialogTitle>
            <DialogContent dividers={true}>
                <FormControl>
                    <FormLabel id="demo-radio-buttons-group-label">{t("saveDialog.saveOptions")}</FormLabel>
                    <RadioGroup
                        aria-labelledby="demo-radio-buttons-group-label"
                        defaultValue="dynamic"
                        name="radio-buttons-group"
                        onChange={(ev => setSelected(ev.target.value))}
                        value={selected}
                    >
                        <FormControlLabel value="dynamic" control={<Radio />} label={t("saveDialog.saveDynamic")} />
                        <FormControlLabel value="static" control={<Radio />} label={t("saveDialog.saveStatic")} />
                    </RadioGroup>
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button onClick={onClose}>{t("saveDialog.cancel")}</Button>
                <Button onClick={saveAndClose} variant="contained" startIcon={<SaveIcon />}>{t("saveDialog.save")}</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SaveDialog;