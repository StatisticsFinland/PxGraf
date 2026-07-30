import * as React from 'react';
import { useTranslation } from 'react-i18next';

import { List, ListSubheader, Container } from '@mui/material';
import { NestedList } from 'components/NestedList/NestedList';
import { styled } from '@mui/material/styles';
import useHierarchyParams from 'hooks/useHierarchyParams';
import { useNavigationContext } from 'contexts/navigationContext';

const TableTreeSelectionWrapper = styled(Container)({ padding: 8 });

const TableSelectionList = styled(List)(({ theme }) => ({
  '& .MuiListItem-root': {
    backgroundColor: theme.palette.background.paper,
  },
})) as typeof List;

const TableSelectionHeader = styled(ListSubheader)(({ theme }) => ({
  backgroundColor: theme.palette.background.default,
  borderBottom: `1px solid ${theme.palette.divider}`,
})) as typeof ListSubheader;

/**
 * Table tree selection view. This is the default view of the program where the user can browse available databases and tables and selects a table for visualization.
 * @see {@link NestedList} component is used for fetching and displaying the database and table hierarchy.
 */
export const TableTreeSelection: React.FC = () => {
  const { t } = useTranslation();
  const { setTablePath } = useNavigationContext();
  const hierarchyParams = useHierarchyParams();

  // Sync URL → context on mount and when URL changes via React Router navigation
  // (e.g. breadcrumb click, browser back/forward). This does NOT fire on folder
  // clicks because those use history.replaceState which bypasses React Router.
  const hierarchyKey = hierarchyParams.join(',');
  React.useEffect(() => {
      setTablePath(prev => {
          const prevKey = prev?.join(',') ?? '';
          if (prevKey === hierarchyKey) return prev;
          return hierarchyKey.length > 0 ? hierarchyKey.split(',') : null;
      });
  }, [setTablePath, hierarchyKey]);

  React.useEffect(() => {
      document.title = `${t("pages.tableTreeSelection")} | PxGraf`;
  }, [t]);

  return (
    <TableTreeSelectionWrapper maxWidth="md">
      <TableSelectionList
        component="nav"
        aria-labelledby="nested-list-subheader"
        subheader={
            <TableSelectionHeader component="div" id="nested-list-subheader">
                {t("tableSelect.title")}
          </TableSelectionHeader>
        }
      >
        <NestedList path={[]} depth={0}/>
      </TableSelectionList>
    </TableTreeSelectionWrapper>
  );
}

export default TableTreeSelection;