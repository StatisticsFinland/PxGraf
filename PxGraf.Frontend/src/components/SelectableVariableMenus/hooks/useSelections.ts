import { useState } from 'react';
import { ISelectableSelections } from '../SelectableDimensionMenus';

const useSelections = (initialSelections: ISelectableSelections = {}) => {
    const [selections, setSelections] = useState(initialSelections);
    return {selections, setSelections};
}

export default useSelections;