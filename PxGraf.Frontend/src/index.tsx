/* istanbul ignore file */

import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import { BrowserRouter } from 'react-router-dom';
import { PublicUrl } from './envVars';

const ROOT = document.getElementById('root');
if (ROOT) {
  const root = ReactDOM.createRoot(ROOT);
    root.render(
        <React.StrictMode>
            <BrowserRouter basename={PublicUrl}>
                <App />
            </BrowserRouter>
        </React.StrictMode>
    );
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
//reportWebVitals();
