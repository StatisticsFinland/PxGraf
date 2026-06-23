/* istanbul ignore file */

import React from 'react';
import { Routes, Route } from "react-router-dom";

import TableListSelection from "views/TableListSelection/TableListSelection";
import TableTreeSelection from "views/TableTreeSelection/TableTreeSelection";
import QueryLoader from "views/QueryLoader/QueryLoader";
import { PageLayout, EditorRoute } from "components/Layout/Layout";

/**
 * URL paths for tables in the editor and table list views.
 * @param {string[]} path - The path to the table
 */
export const urls = {
    editor: (path: string[]) => {
        return `/editor/${path.join("/")}/`;
    },
    tableTree: `/`,
    tableList: (path: string[]) => {
        return `/table-list/${path.join("/")}/`;
    }
}

export function Router() {
    return (
        <Routes>
            <Route path={"/"} element={<PageLayout element={<TableTreeSelection />} />} />
            <Route path={"/editor/*"} element={<EditorRoute />} />
            <Route path={"/table-list/*"} element={<PageLayout element={<TableListSelection />} />} />
            <Route path={"/sqid/*"} element={<PageLayout element={<QueryLoader />} />} />
        </Routes>
    );
}
