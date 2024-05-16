import { useState } from 'react';
import { IMenuProps } from '../SelectableVariableMenus';

const useSelections = (initialSelections: IMenuProps = {}) => {
  const [selections, setSelections] = useState(initialSelections);

  return {selections, setSelections};
}

export default useSelections;