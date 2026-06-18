/* istanbul ignore file */

import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
import { BasePath } from './envVars';

const ROOT = document.getElementById('root');
if (ROOT) {
    const root = ReactDOM.createRoot(ROOT);
    root.render(
        <React.StrictMode>
            <BrowserRouter basename={BasePath}>
                <App />
            </BrowserRouter>
        </React.StrictMode>
    );
}

