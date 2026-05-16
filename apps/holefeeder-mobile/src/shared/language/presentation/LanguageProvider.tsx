import React, { createContext, useEffect, useState } from 'react';
import { getI18nInstance, initI18n } from '@/i18n';
import { Logger } from '@/shared/core/logger/logger';
import { LanguageState } from '@/shared/language/core/language-state';
import { LanguageType } from '@/shared/language/core/language-type';
import { AppStorage } from '@/shared/persistence/app-storage';

const logger = Logger.create('LanguageProvider');

const APP_SETTINGS_LANGUAGE_KEY = 'app-settings-language';

const availableLanguages = [
  { code: 'en' as LanguageType, name: 'English' },
  { code: 'fr' as LanguageType, name: 'Français' },
];

export const LanguageContext = createContext<LanguageState | undefined>(undefined);

export const LanguageProvider = ({ children, storage }: { children: React.ReactNode; storage: AppStorage }) => {
  const [language, setLanguage] = useState<LanguageType>(
    () => (storage.getString(APP_SETTINGS_LANGUAGE_KEY) as LanguageType) || LanguageType.en
  );
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    const init = async () => {
      try {
        await initI18n(language);
        setIsInitialized(true);
      } catch (error) {
        logger.error('Failed to initialize language:', error);
      }
    };
    init();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Initialize only once on mount

  useEffect(() => {
    if (!isInitialized) return;

    const updateLanguage = async () => {
      storage.setString(APP_SETTINGS_LANGUAGE_KEY, language);
      try {
        await getI18nInstance().changeLanguage(language);
      } catch (error) {
        logger.error('Failed to change language:', error);
      }
    };

    updateLanguage();
  }, [isInitialized, language, storage]);

  if (!isInitialized) {
    logger.info('Waiting for language to initialize...');
    return <></>;
  }

  const value: LanguageState = {
    language,
    setLanguage,
    availableLanguages,
  };

  return <LanguageContext.Provider value={value}>{children}</LanguageContext.Provider>;
};
