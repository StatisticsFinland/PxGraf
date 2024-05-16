/* istanbul ignore file */

import {
  Routes,
  Route,
} from "react-router-dom";

import { EditorProvider } from "contexts/editorContext";
import Editor from "views/Editor/Editor";
import TableListSelection from "views/TableListSelection/TableListSelection";
import TableTreeSelection from "views/TableTreeSelection/TableTreeSelection";
import QueryLoader from "views/QueryLoader/QueryLoader";
import React, { ReactNode } from 'react';
import Header from "components/Header/Header";
import { Divider } from "@mui/material";

/**
 * URL paths for tables in the editor and table list views.
 * @param {string[]} path - The path to the table
 */
export const urls = {
  editor: (path: string[]) => {
    return "/" + ["editor", ...path].join("/") + "/";
  },
  tableTree: "/",
  tableList: (path: string[]) => {
    return "/" + ["table-list", ...path].join("/")  + "/";
  }
}

const PageLayout: React.FC<{ element: ReactNode }> = ({ element }) => (
    <>
        <Header />
        <Divider />
        {element}
    </>
)

export function Router() {
    return (
        <Routes>
            <Route path="/" element={<PageLayout element={<TableTreeSelection />} />} />
            <Route path={"/editor/*"} element={<PageLayout element={<EditorProvider><Editor /></EditorProvider>} />} />
            <Route path={"/table-list/*"} element={<PageLayout element={<TableListSelection />} />} />
            <Route path={"/sqid/*"} element={<PageLayout element={<QueryLoader />} />} />
        </Routes>
    );
}
