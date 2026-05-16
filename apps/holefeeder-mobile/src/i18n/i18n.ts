import * as Localization from 'expo-localization';
import i18next from 'i18next';
import { initReactI18next } from 'react-i18next';
import { Logger } from '@/shared/core/logger/logger';
import { en } from './locales/en-CA/translations';
import { fr } from './locales/fr-CA/translations';

const log = Logger.create('i18n');

const resources = {
  en: {
    translation: en,
  },
  fr: {
    translation: fr,
  },
};

let isInitialized = false;
const instance = i18next;

export const initI18n = async (savedLanguage?: string): Promise<void> => {
  if (isInitialized) {
    return;
  }

  try {
    // If no saved language provided, try AsyncStorage then device locale
    let languageToUse = savedLanguage;

    // If still no language, use device locale or fallback to English
    if (!languageToUse) {
      const deviceLanguage = Localization.getLocales()[0].languageCode ?? '';
      const supportedLanguages = ['en', 'fr'];
      languageToUse = supportedLanguages.includes(deviceLanguage) ? deviceLanguage : 'en';
    }

    await instance.use(initReactI18next).init({
      resources,
      lng: languageToUse,
      fallbackLng: {
        'en-*': ['en'],
        'fr-*': ['fr', 'en'],
        default: ['en'],
      },

      interpolation: {
        escapeValue: false, // React already escapes values
      },

      compatibilityJSON: 'v4', // Using v4 format as expected by react-i18next
    });

    isInitialized = true;
  } catch (error) {
    log.error('Error initializing i18n:', error);
    // Fallback initialization without saved language
    await instance.use(initReactI18next).init({
      resources,
      lng: 'en',
      fallbackLng: 'en',
      interpolation: {
        escapeValue: false,
      },
      compatibilityJSON: 'v4',
    });
    isInitialized = true;
  }
};

export const getI18nInstance = () => {
  if (!isInitialized) {
    log.warn('i18n not initialized yet. Call initI18n() first.');
  }
  return instance;
};
