/* istanbul ignore file */

import React from 'react';
import {
  QueryClient,
  QueryClientProvider,
} from 'react-query';
import './App.css';

import {
  CssBaseline, Box, ThemeProvider
} from '@mui/material';

import { UiLanguageProvider } from "contexts/uiLanguageContext";
import { Router } from "Router";

import './i18n';
import styled from 'styled-components';
import { NavigationProvider } from 'contexts/navigationContext';
import theme from 'styles/materialTheme';

const BodyWrapper = styled(Box)`
  min-height: 100vh;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: auto auto 1fr;
`;

const queryClient = new QueryClient()

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <UiLanguageProvider>
        <NavigationProvider>
          <ThemeProvider theme={theme}>
            <CssBaseline />
            <BodyWrapper>
              <Router />
            </BodyWrapper>
          </ThemeProvider>
        </NavigationProvider>
      </UiLanguageProvider>
    </QueryClientProvider>)
}

export default App
