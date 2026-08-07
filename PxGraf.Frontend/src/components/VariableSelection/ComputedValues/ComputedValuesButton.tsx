import React from 'react';
import { Badge, IconButton, Tooltip } from '@mui/material';
import FunctionsIcon from '@mui/icons-material/Functions';
import { useTranslation } from 'react-i18next';
import { IDimension } from 'types/cubeMeta';
import { IDimensionQuery } from 'types/query';
import { removeDeletedVirtualCodesFromItemFilter } from 'utils/virtualValueHelpers';
import ComputedValuesDialog from './ComputedValuesDialog';

interface ComputedValuesButtonProps {
    dimension: IDimension;
    dimensionQuery: IDimensionQuery;
    onQueryChanged: (newDimensionQuery: IDimensionQuery) => void;
}

export const ComputedValuesButton: React.FC<ComputedValuesButtonProps> = ({
    dimension,
    dimensionQuery,
    onQueryChanged,
}) => {
    const { t } = useTranslation();
    const [dialogOpen, setDialogOpen] = React.useState(false);

    const handleQueryChanged = (newDimensionQuery: IDimensionQuery) => {
        // Remove any deleted virtual codes from an active FilterType.Item filter
        const cleanedDimensionQuery = removeDeletedVirtualCodesFromItemFilter(
            dimensionQuery.virtualValueDefinitions,
            newDimensionQuery,
        );

        // Forward the dimension query update to the parent
        onQueryChanged(cleanedDimensionQuery);
    };

    const definitionCount = dimensionQuery.virtualValueDefinitions.length;
    const buttonLabel = definitionCount > 0
        ? t('computedValues.buttonWithCount', { count: definitionCount })
        : t('computedValues.button');

    return (
        <>
            <Tooltip title={buttonLabel}>
                <Badge
                    badgeContent={definitionCount > 0 ? definitionCount : undefined}
                    color="primary"
                    sx={{ '& .MuiBadge-badge': { fontSize: 11, height: 16, minWidth: 16, padding: '0 3px', transform: 'scale(1) translate(35%, -35%)' } }}
                >
                    <IconButton
                        aria-label={buttonLabel}
                        color={definitionCount > 0 ? 'primary' : 'default'}
                        size="small"
                        onClick={() => setDialogOpen(true)}
                    >
                        <FunctionsIcon />
                    </IconButton>
                </Badge>
            </Tooltip>
            <ComputedValuesDialog
                open={dialogOpen}
                dimension={dimension}
                dimensionQuery={dimensionQuery}
                onClose={() => setDialogOpen(false)}
                onQueryChanged={handleQueryChanged}
            />
        </>
    );
};

export default ComputedValuesButton;
