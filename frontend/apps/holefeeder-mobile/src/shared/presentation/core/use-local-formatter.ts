import { getLocales } from 'expo-localization';
import { useEffect, useRef, useState } from 'react';
import { AppState, AppStateStatus } from 'react-native';

type LocaleHook = {
  currentLocale: string;
  currencyCode: string;
};

export const useLocaleFormatter = (): LocaleHook => {
  const [localeInfo, setLocaleInfo] = useState(getLocales()[0]);
  const appState = useRef(AppState.currentState);

  useEffect(() => {
    const subscription = AppState.addEventListener('change', (nextAppState: AppStateStatus) => {
      if (appState.current.match(/inactive|background/) && nextAppState === 'active') {
        const newLocale = getLocales()[0];

        if (newLocale.languageTag !== localeInfo.languageTag) {
          setLocaleInfo(newLocale);
        }
      }
      appState.current = nextAppState;
    });

    return () => {
      subscription.remove();
    };
  }, [localeInfo.languageTag]);

  return {
    currentLocale: localeInfo.languageTag,
    currencyCode: localeInfo.currencyCode ?? 'CAD',
  };
};
