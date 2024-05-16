import * as React from 'react';
import { useTranslation } from 'react-i18next';

import { List, ListSubheader, Container } from '@mui/material';
import { NestedList } from 'components/NestedList/NestedList';
import styled from 'styled-components';
import InfoBubble from 'components/InfoBubble/InfoBubble';
import useReplaceQueryParams from 'hooks/useReplaceQueryParams';

const TableTreeSelectionWrapper = styled(Container)`
  padding: 8px;
`;

const SubHeaderWrapper = styled.div`
  display: flex;
`;

/**
 * Table tree selection view. This is the default view of the program where the user can browse available databases and tables and selects a table for visualization.
 * @see {@link NestedList} component is used for fetching and displaying the database and table hierarchy.
 */
export const TableTreeSelection: React.FC = () => {
  const { t } = useTranslation();
  useReplaceQueryParams('/');

  React.useEffect(() => {
      document.title = `${t("pages.tableTreeSelection")} | PxGraf`;
  }, []);

  return (
    <TableTreeSelectionWrapper maxWidth="md">
      <List
        component="nav"
        aria-labelledby="nested-list-subheader"
        subheader={
            <ListSubheader component="div" id="nested-list-subheader">
                <SubHeaderWrapper><div>{t("tableSelect.title")}</div><InfoBubble info={t("infoText.treeSelection")} ariaLabel={t("tableSelect.title")} id="mainContent" /></SubHeaderWrapper>
          </ListSubheader>
        }
      >
        <NestedList path={[]} depth={0}/>
      </List>
    </TableTreeSelectionWrapper>
  );
}

export default TableTreeSelection;