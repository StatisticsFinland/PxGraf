/* istanbul ignore file */

import i18n from "i18next";
import detector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";

// Extends the NodeRequire interface to allow require.context to be used in TypeScript
declare const require: NodeRequire;
interface NodeRequire {
    context: (directory: string, useSubdirectories?: boolean, regExp?: RegExp) => any;
}

// Import all JSON files from the localization folder
const localizationContext = require.context('./localization', true, /\.json$/);

// Search for available languages from the translationsConfig.json file
const translationsConfigFile = './translationsConfig.json';
const translationsConfig = localizationContext.keys().includes(translationsConfigFile)
    ? localizationContext(translationsConfigFile)
    : null;

// If not found, use all JSON files in the localization folder as translations
const availableLanguages = translationsConfig?.availableTranslations
    ? translationsConfig.availableTranslations
    : localizationContext.keys().map(file => file.replace('./', '').replace('.json', ''));

// Iterate through available languages and create a resource object of the languages
const resources = availableLanguages.reduce((acc, lang) => { 
    const file = `./${lang}.json`;
    if (localizationContext.keys().includes(file) && file !== translationsConfig) {
        acc[lang] = {
            translation: localizationContext(file),
        };
    }
    return acc;
}, {});

i18n.on('languageChanged', (lng) => {document.documentElement.setAttribute('lang', lng);});

i18n
  .use(detector)
  .use(initReactI18next)
  .init({
    resources,
    fallbackLng: availableLanguages[0],
    interpolation: {
      escapeValue: false, //Html encoding is handled by react
    }
  });

  export default i18n;