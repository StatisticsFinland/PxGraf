import React from 'react';
import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    FormControl,
    FormLabel,
    IconButton,
    List,
    ListItem,
    ListItemText,
    Stack,
    ToggleButton,
    ToggleButtonGroup,
    Tooltip,
    Typography,
} from '@mui/material';
import LinkIcon from '@mui/icons-material/Link';
import DeleteIcon from '@mui/icons-material/Delete';
import EditIcon from '@mui/icons-material/Edit';
import { useTranslation } from 'react-i18next';
import { IDimension, IDimensionValue } from 'types/cubeMeta';
import { IDimensionQuery, IVirtualValueDefinition, VirtualValueOperator } from 'types/query';
import { UiLanguageContext } from 'contexts/uiLanguageContext';
import { generateVirtualValueCode, getOperandCodes, getOperatorType, getConstant, buildDefinition } from 'utils/virtualValueHelpers';
import SumOperationForm from './SumOperationForm';
import SubtractionOperationForm from './SubtractionOperationForm';
import MultiplicationOperationForm from './MultiplicationOperationForm';
import DivisionOperationForm from './DivisionOperationForm';

const operatorFormComponents: Record<VirtualValueOperator, React.ComponentType<{
    availableValues: IDimensionValue[];
    operandCodes: string[];
    constant: number | undefined;
    onChange: (operandCodes: string[], constant?: number) => void;
}>> = {
    sum: SumOperationForm,
    subtraction: SubtractionOperationForm,
    multiplication: MultiplicationOperationForm,
    division: DivisionOperationForm,
};

interface ComputedValuesDialogProps {
    open: boolean;
    dimension: IDimension;
    dimensionQuery: IDimensionQuery;
    onClose: () => void;
    onQueryChanged: (newDimensionQuery: IDimensionQuery) => void;
}

function resolveValueName(code: string, dimension: IDimension, uiContentLanguage: string): string {
    const real = dimension.values.find(v => v.code === code);
    if (real) {
        return real.name[uiContentLanguage] ?? code;
    }
    return code;
}

function operatorSymbol(def: IVirtualValueDefinition): string {
    switch (getOperatorType(def)) {
        case 'sum': return '+';
        case 'subtraction': return '-';
        case 'multiplication': return '×';
        case 'division': return '÷';
    }
}

function operatorLabel(operator: VirtualValueOperator, t: (key: string) => string): string {
    switch (operator) {
        case 'sum': return t('computedValues.operatorSum');
        case 'subtraction': return t('computedValues.operatorSubtraction');
        case 'multiplication': return t('computedValues.operatorMultiplication');
        case 'division': return t('computedValues.operatorDivision');
    }
}

interface DefinitionListViewProps {
    definitions: IVirtualValueDefinition[];
    dimension: IDimension;
    uiContentLanguage: string;
    dependedOnCodes: Set<string>;
    onEdit: (definition: IVirtualValueDefinition) => void;
    onDelete: (code: string) => void;
    onAddNew: () => void;
    t: (key: string) => string;
}

const DefinitionListView: React.FC<DefinitionListViewProps> = ({
    definitions, dimension, uiContentLanguage, dependedOnCodes, onEdit, onDelete, onAddNew, t,
}) => (
    <>
        {definitions.length === 0 ? (
            <Typography variant="body2" color="text.secondary">
                {t('computedValues.noValues')}
            </Typography>
        ) : (
            <List disablePadding>
                {definitions.map(def => {
                    const definitionName = resolveValueName(def.code, dimension, uiContentLanguage);
                    const operandNames = getOperandCodes(def)
                        .map(code => resolveValueName(code, dimension, uiContentLanguage))
                        .join(', ');
                    const defConstant = getConstant(def);
                    const constantPart = defConstant === undefined ? '' : ` ${operatorSymbol(def)} ${defConstant}`;
                    return (
                        <ListItem
                            key={def.code}
                            disablePadding
                            sx={{ py: 0.5, alignItems: 'flex-start' }}
                        >
                            <ListItemText
                                primary={`${definitionName} — ${operatorLabel(getOperatorType(def), t)}`}
                                secondary={`${operandNames}${constantPart}`}
                                sx={{ flex: 1, minWidth: 0 }}
                            />
                            <Stack direction="row" spacing={0.5} alignItems="center" sx={{ flexShrink: 0, ml: 1 }}>
                                    {dependedOnCodes.has(def.code) && (
                                        <Tooltip title={t('computedValues.hasDependents')}>
                                            <span style={{ display: 'inline-flex', alignItems: 'center', gap: 4 }}>
                                                <LinkIcon fontSize="small" color="action" data-testid="dependency-indicator" />
                                                <Typography component="span" variant="caption" color="text.secondary">
                                                    {t('computedValues.hasDependents')}
                                                </Typography>
                                            </span>
                                        </Tooltip>
                                    )}
                                    <span style={{ display: 'inline-flex' }}>
                                        <IconButton
                                            edge="end"
                                            aria-label={`${t('computedValues.edit')}: ${definitionName}`}
                                            onClick={() => onEdit(def)}
                                            size="small"
                                            disabled={dependedOnCodes.has(def.code)}
                                        >
                                            <EditIcon fontSize="small" />
                                        </IconButton>
                                    </span>
                                    <span style={{ display: 'inline-flex' }}>
                                        <IconButton
                                            edge="end"
                                            aria-label={`${t('computedValues.delete')}: ${definitionName}`}
                                            onClick={() => onDelete(def.code)}
                                            size="small"
                                            disabled={dependedOnCodes.has(def.code)}
                                        >
                                            <DeleteIcon fontSize="small" />
                                        </IconButton>
                                    </span>
                                </Stack>
                        </ListItem>
                    );
                })}
            </List>
        )}
        <Button
            variant="outlined"
            onClick={onAddNew}
            sx={{ mt: 2 }}
        >
            {t('computedValues.addNew')}
        </Button>
    </>
);

interface OperatorFormViewProps {
    formOperator: VirtualValueOperator;
    editingDefinition: IVirtualValueDefinition | undefined;
    availableValues: IDimensionValue[];
    formOperandCodes: string[];
    formConstant: number | undefined;
    validationError: string;
    onOperatorChange: (_: React.MouseEvent<HTMLElement>, value: VirtualValueOperator | null) => void;
    onFormChange: (operandCodes: string[], constant?: number) => void;
    t: (key: string) => string;
}

const OperatorFormView: React.FC<OperatorFormViewProps> = ({
    formOperator, editingDefinition, availableValues, formOperandCodes, formConstant,
    validationError, onOperatorChange, onFormChange, t,
}) => {
    const OperatorForm = operatorFormComponents[formOperator];
    const validationErrorId = 'computed-values-validation-error';
    return (
        <Stack spacing={2} role="group" aria-describedby={validationError !== '' ? validationErrorId : undefined}>
            <FormControl>
                <FormLabel>{t('computedValues.operator')}</FormLabel>
                <ToggleButtonGroup
                    exclusive
                    size="small"
                    color="primary"
                    value={formOperator}
                    onChange={onOperatorChange}
                    aria-label={t('computedValues.operator')}
                    sx={{ mt: 1, flexWrap: 'wrap' }}
                >
                    <ToggleButton value="sum">{t('computedValues.operatorSum')}</ToggleButton>
                    <ToggleButton value="subtraction">{t('computedValues.operatorSubtraction')}</ToggleButton>
                    <ToggleButton value="multiplication">{t('computedValues.operatorMultiplication')}</ToggleButton>
                    <ToggleButton value="division">{t('computedValues.operatorDivision')}</ToggleButton>
                </ToggleButtonGroup>
            </FormControl>
            <OperatorForm
                key={`${formOperator}-${editingDefinition?.code ?? 'new'}`}
                availableValues={availableValues}
                operandCodes={formOperandCodes}
                constant={formConstant}
                onChange={onFormChange}
            />
            {validationError !== '' && (
                <Typography id={validationErrorId} role="alert" variant="body2" color="error">
                    {validationError}
                </Typography>
            )}
        </Stack>
    );
};

export const ComputedValuesDialog: React.FC<ComputedValuesDialogProps> = ({
    open,
    dimension,
    dimensionQuery,
    onClose,
    onQueryChanged,
}) => {
    const { t } = useTranslation();
    const { uiContentLanguage } = React.useContext(UiLanguageContext);

    const [view, setView] = React.useState<'list' | 'form'>('list');
    const [editingDefinition, setEditingDefinition] = React.useState<IVirtualValueDefinition | undefined>(undefined);
    const [formOperator, setFormOperator] = React.useState<VirtualValueOperator>('sum');
    const [formOperandCodes, setFormOperandCodes] = React.useState<string[]>([]);
    const [formConstant, setFormConstant] = React.useState<number | undefined>(undefined);
    const [validationError, setValidationError] = React.useState<string>('');
    const dialogTitleRef = React.useRef<HTMLHeadingElement>(null);

    React.useEffect(() => {
        if (open) {
            dialogTitleRef.current?.focus();
        }
    }, [open, view]);

    const handleAddNew = () => {
        setEditingDefinition(undefined);
        setFormOperator('sum');
        setFormOperandCodes([]);
        setFormConstant(undefined);
        setValidationError('');
        setView('form');
    };

    const handleEdit = (definition: IVirtualValueDefinition) => {
        setEditingDefinition(definition);
        setFormOperator(getOperatorType(definition));
        setFormOperandCodes(getOperandCodes(definition));
        setFormConstant(getConstant(definition));
        setValidationError('');
        setView('form');
    };

    const handleDelete = (code: string) => {
        onQueryChanged({
            ...dimensionQuery,
            virtualValueDefinitions: dimensionQuery.virtualValueDefinitions.filter(def => def.code !== code),
        });
    };

    const handleCancel = () => {
        setView('list');
        setEditingDefinition(undefined);
        setValidationError('');
    };

    const handleFormChange = (operandCodes: string[], constant?: number) => {
        setFormOperandCodes(operandCodes);
        setFormConstant(constant);
    };

    const handleOperatorChange = (_: React.MouseEvent<HTMLElement>, value: VirtualValueOperator | null) => {
        if (value === null) return;
        setFormOperator(value);
        setFormOperandCodes([]);
        setFormConstant(undefined);
        setValidationError('');
    };

    const validateNonSumOperands = (): boolean => {
        if (formOperandCodes.length === 0 || formOperandCodes[0] === '') {
            setValidationError(t('computedValues.validationSelectValue'));
            return false;
        }
        if (formConstant === undefined) {
            if (formOperandCodes.length < 2 || formOperandCodes[1] === '') {
                setValidationError(t('computedValues.validationSelectValue'));
                return false;
            }
            if (formOperandCodes[0] === formOperandCodes[1]) {
                setValidationError(t('computedValues.validationSelectValue'));
                return false;
            }
        } else if (formOperator === 'division' && formConstant === 0) {
            setValidationError(t('computedValues.validationDivisionByZero'));
            return false;
        }
        return true;
    };

    const validate = (): boolean => {
        if (formOperator === 'sum') {
            const minOperands = formConstant === undefined ? 2 : 1;
            if (formOperandCodes.length < minOperands) {
                setValidationError(t('computedValues.validationMinOperands'));
                return false;
            }
        } else if (!validateNonSumOperands()) {
            return false;
        }
        setValidationError('');
        return true;
    };

    const handleSave = () => {
        if (!validate()) {
            return;
        }

        const isEditing = editingDefinition !== undefined;
        const code = isEditing
            ? editingDefinition.code
            : generateVirtualValueCode(dimensionQuery.virtualValueDefinitions, formOperator);

        const newDefinition: IVirtualValueDefinition = buildDefinition(code, formOperator, formOperandCodes, formConstant);

        const updatedDefinitions = isEditing
            ? dimensionQuery.virtualValueDefinitions.map(def =>
                def.code === editingDefinition.code ? newDefinition : def,
            )
            : [...dimensionQuery.virtualValueDefinitions, newDefinition];

        onQueryChanged({
            ...dimensionQuery,
            virtualValueDefinitions: updatedDefinitions,
        });

        setView('list');
        setEditingDefinition(undefined);
    };

    // Build available values: filter dimension.values to exclude the one being edited
    const availableValues: IDimensionValue[] = dimension.values
        .filter(v => !(v.isVirtual && v.code === editingDefinition?.code))
        .map(v => ({ ...v }));

    const definitions = dimensionQuery.virtualValueDefinitions;
    const allDefinitionCodes = new Set(definitions.map(d => d.code));
    const dependedOnCodes = new Set(
        definitions.flatMap(d => getOperandCodes(d)).filter(c => allDefinitionCodes.has(c))
    );

    const getDialogTitle = (): string => {
        if (view === 'list') return t('computedValues.dialogTitle');
        if (editingDefinition === undefined) return t('computedValues.addNew');
        return t('computedValues.editTitle');
    };

    return (
        <Dialog
            open={open}
            onClose={onClose}
            scroll="paper"
            fullWidth
            maxWidth="sm"
            aria-labelledby="computed-values-dialog-title"
            slotProps={{ transition: { onExited: handleCancel } }}
        >
            <DialogTitle id="computed-values-dialog-title" ref={dialogTitleRef} tabIndex={-1}>
                {getDialogTitle()}
            </DialogTitle>
            <DialogContent dividers>
                {view === 'list' ? (
                    <DefinitionListView
                        definitions={definitions}
                        dimension={dimension}
                        uiContentLanguage={uiContentLanguage}
                        dependedOnCodes={dependedOnCodes}
                        onEdit={handleEdit}
                        onDelete={handleDelete}
                        onAddNew={handleAddNew}
                        t={t}
                    />
                ) : (
                    <OperatorFormView
                        formOperator={formOperator}
                        editingDefinition={editingDefinition}
                        availableValues={availableValues}
                        formOperandCodes={formOperandCodes}
                        formConstant={formConstant}
                        validationError={validationError}
                        onOperatorChange={handleOperatorChange}
                        onFormChange={handleFormChange}
                        t={t}
                    />
                )}
            </DialogContent>
            <DialogActions>
                {view === 'list' ? (
                    <Button onClick={onClose}>{t('computedValues.close')}</Button>
                ) : (
                    <>
                        <Button onClick={handleCancel}>{t('computedValues.cancel')}</Button>
                        <Button onClick={handleSave} variant="contained">
                            {t('computedValues.save')}
                        </Button>
                    </>
                )}
            </DialogActions>
        </Dialog>
    );
};

export default ComputedValuesDialog;
