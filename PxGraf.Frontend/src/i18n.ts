/* istanbul ignore file */

import i18n from "i18next";
import detector from "i18next-browser-languagedetector";
import { initReactI18next } from "react-i18next";

interface TranslationContent {
    [key: string]: any;
}

interface TranslationsConfig {
    availableTranslations: string[];
}

// Import all JSON files from the localization folder
const localizationFiles: Record<string, { default: TranslationContent }> = import.meta.glob('./localization/*.json', {eager: true});

async function loadTranslations() {
    // Check if translationsConfig.json is among the imported files
    const translationsConfigPath: string = './localization/translationsConfig.json';
    const translationsConfig: TranslationsConfig | null =
        localizationFiles[translationsConfigPath]?.default as TranslationsConfig ??
        null;

    // Define available languages primarily from translationsConfig.json, otherwise from the imported file names
    const availableLanguages = translationsConfig?.availableTranslations ?? Object.keys(localizationFiles)
        .map(file => file.replace('./localization/', '').replace('.json', ''))
        .filter(lang => lang !== 'translationsConfig');

    // Load translations for available languages
    const resources = Object.fromEntries(
        availableLanguages.map(lang => [
            lang,
            { translation: localizationFiles[`./localization/${lang}.json`]?.default },
        ])
    );

    return { resources, availableLanguages };
}

async function initI18n() {
    const { resources, availableLanguages } = await loadTranslations();
    i18n.on('languageChanged', (lng) => { document.documentElement.setAttribute('lang', lng); });

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
}

initI18n();
export default i18n;