import React from 'react';
import { Badge, Button } from '@mui/material';
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

    return (
        <>
            <Badge badgeContent={definitionCount > 0 ? definitionCount : undefined} color="primary">
                <Button
                    variant="outlined"
                    size="small"
                    onClick={() => setDialogOpen(true)}
                >
                    {t('computedValues.button')}
                </Button>
            </Badge>
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
