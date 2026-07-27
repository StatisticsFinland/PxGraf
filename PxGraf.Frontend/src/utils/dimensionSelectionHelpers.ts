import { FilterType, IValueFilter } from "types/query";

export const getDefaultFilter: (fType: FilterType) => IValueFilter = (filterType: FilterType) => {
  switch(filterType) {
    case FilterType.Item:
      return { type: FilterType.Item, query: [] };
    case FilterType.All:
      return { type: FilterType.All };
    case FilterType.From:
      return { type: FilterType.From, query: null };
    case FilterType.Top:
      return { type: FilterType.Top, query: 1 };
    case FilterType.InverseItem:
      return { type: FilterType.InverseItem, query: [] };
    case FilterType.Regex:
      return { type: FilterType.Regex, query: '' };
  }
}

export const queryTypeLabels: { [key in FilterType] : string } = {
  [FilterType.All]: "variableSelect.allFilter",
  [FilterType.From]: "variableSelect.fromFilter",
  [FilterType.Top]: "variableSelect.topFilter",
  [FilterType.Item]: "variableSelect.itemFilter",
  [FilterType.InverseItem]: "variableSelect.inverseItemFilter",
  [FilterType.Regex]: "variableSelect.regexFilter"
}