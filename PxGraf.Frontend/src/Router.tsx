/* istanbul ignore file */

import React from 'react';
import { Routes, Route } from "react-router-dom";

import TableTreeSelection from "views/TableTreeSelection/TableTreeSelection";
import QueryLoader from "views/QueryLoader/QueryLoader";
import { PageLayout, EditorRoute } from "components/Layout/Layout";

export function Router() {
    return (
        <Routes>
            <Route path={"/"} element={<PageLayout element={<TableTreeSelection />} />} />
            <Route path={"/editor/*"} element={<EditorRoute />} />
            <Route path={"/sqid/*"} element={<PageLayout element={<QueryLoader />} />} />
        </Routes>
    );
}
