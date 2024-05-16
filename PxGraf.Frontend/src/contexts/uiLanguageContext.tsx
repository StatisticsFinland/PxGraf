/* istanbul ignore file */

import * as React from 'react';
import { useTranslation } from 'react-i18next';

/**
 * Interface for UI language context properties
 * @property language - The current UI language. These languages are defined in ../localization/translationsConfig.json.
 * @property setLanguage - Function to set the UI language
 * @property languageTab - The current content language. These languages are defined by the content retrieved from the API and are used to customize the visualization.
 * @property setLanguageTab - Function to set the content language
 * @property availableUiLanguages - List of available UI languages
 * @property uiContentLanguage - The current UI content language. These languages are defined by the content retrieved from the API, but are used in the UI.
 * @property setUiContentLanguage - Function to set the UI content language
 */
interface IUiLanguageContext {
    language: string;
    setLanguage: React.Dispatch<string>;
    languageTab: string;
    setLanguageTab: React.Dispatch<string>;
    availableUiLanguages: string[];
    uiContentLanguage: string;
    setUiContentLanguage: React.Dispatch<string>;
}

export const UiLanguageContext = React.createContext<IUiLanguageContext>({
    language: "",
    setLanguage: () => { /* no base implementation */ },
    languageTab: '',
    setLanguageTab: () => { /* no base implementation */ },
    availableUiLanguages: [],
    uiContentLanguage: '',
    setUiContentLanguage: () => { /* no base implementation */ }
});

/**
 * Provider for the UI language context
 * @param children - The child components
 */
export const UiLanguageProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { i18n } = useTranslation();
    const [language, setLanguage] = React.useState(i18n.language);
    const [languageTab, setLanguageTab] = React.useState(i18n.language);
    const [availableUiLanguages, setAvailableUiLanguages] = React.useState<string[]>([]);
    const [uiContentLanguage, setUiContentLanguage] = React.useState(i18n.language);

    const loadLanguages = async () => {
        // Load available languages from i18n
        const languages = Object.keys(i18n.services.resourceStore.data);
        setAvailableUiLanguages(languages);
        const defaultLanguage = languages.includes(i18n.language) ? i18n.language : languages[0];
        if (language !== defaultLanguage) {
            setLanguage(defaultLanguage);
        }
    }

    React.useEffect(() => {
        loadLanguages();
    }, []);

    const setUiLang = (lang: string) => {
        i18n.changeLanguage(lang);
        setLanguage(lang);
    }

    const contextValue = React.useMemo(() => {
        return { language, setLanguage: setUiLang, languageTab, setLanguageTab, availableUiLanguages, uiContentLanguage, setUiContentLanguage }
    }, [language, languageTab, availableUiLanguages, uiContentLanguage]);

    return (
        <UiLanguageContext.Provider value={contextValue}>
            {children}
        </UiLanguageContext.Provider>
    );
}

export default UiLanguageContext;
