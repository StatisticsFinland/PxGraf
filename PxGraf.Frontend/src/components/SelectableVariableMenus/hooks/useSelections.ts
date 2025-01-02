import { useState } from 'react';
import { IMenuProps } from '../SelectableDimensionMenus';

const useSelections = (initialSelections: IMenuProps = {}) => {
  const [selections, setSelections] = useState(initialSelections);

  return {selections, setSelections};
}

export default useSelections;