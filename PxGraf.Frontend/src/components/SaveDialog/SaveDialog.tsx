import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
    FormControlLabel, FormControl, Button, Dialog, DialogTitle,
    DialogContent, DialogActions, FormLabel, RadioGroup, Radio, Checkbox
} from '@mui/material';
import SaveIcon from '@mui/icons-material/Save';
import { EditorContext } from '../../contexts/editorContext';

interface ISaveDialogProps {
    onSave: (archive: boolean, draft: boolean) => void;
}

/**
 * SaveDialog component for saving queries. Contains options to save the current query as either dynamic (with updating data) or static (with fixed data), and whether to save it as a draft or published.
 * @param {ISaveDialogProps} props - The properties for the SaveDialog component.
 * @param {function} props.onSave - Callback function to handle the save action.
 * @returns {JSX.Element} The rendered SaveDialog component.
 */
export const SaveDialog: React.FC<ISaveDialogProps> = ({ onSave }) => {
    const { t } = useTranslation();
    const [selected, setSelected] = useState("dynamic");
    const { saveDialogOpen, setSaveDialogOpen, publicationWebhookEnabled } = React.useContext(EditorContext);
    const [saveAsPublished, setSaveAsPublished] = useState(false);

    const handleDraftChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setSaveAsPublished(event.target.checked);
    };

    const saveAndClose = () => {
        // When publication is disabled, always save as published (non-draft)
        const isDraft = publicationWebhookEnabled ? !saveAsPublished : false;
        onSave(selected === "static", isDraft);
        setSaveDialogOpen(false);
    };

    return (
        <Dialog
            open={saveDialogOpen}
            onClose={() => setSaveDialogOpen(false)}
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
                    {publicationWebhookEnabled && (
                        <FormControlLabel
                            control={
                                <Checkbox
                                    checked={saveAsPublished}
                                    onChange={handleDraftChange}
                                />
                            }
                            label={t("saveDialog.publish")}
                        />
                    )}
                </FormControl>
            </DialogContent>
            <DialogActions>
                <Button onClick={() => setSaveDialogOpen(false)}>{t("saveDialog.cancel")}</Button>
                <Button onClick={saveAndClose} variant="contained" startIcon={<SaveIcon />}>{t("saveDialog.save")}</Button>
            </DialogActions>
        </Dialog>
    );
}

export default SaveDialog;