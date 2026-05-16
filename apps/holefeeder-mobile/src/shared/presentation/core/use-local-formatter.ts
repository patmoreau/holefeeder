import { differenceInCalendarDays, parseISO } from 'date-fns';
import { getLocales } from 'expo-localization';
import { useEffect, useMemo, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { AppState, AppStateStatus } from 'react-native';
import { tk } from '@/i18n/translations';
import { DateOnly } from '@/shared/core/date-only';
import { withDate } from '@/shared/core/with-date';

type FormatterHook = {
  currentLocale: string;
  currencyCode: string;
  formatCurrency: (amount: number, options?: { currency?: string; isEditing?: boolean }) => string;
  formatDate: (date: DateOnly, anchorDate: DateOnly, options?: Intl.DateTimeFormatOptions) => string;
  formatPercentage: (val: number) => string;
};

export const useLocaleFormatter = (): FormatterHook => {
  const { t } = useTranslation();
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

  const helpers = useMemo(
    () => ({
      formatCurrency: (amount: number, options?: { currency?: string; isEditing?: boolean }) => {
        const { currency = localeInfo.currencyCode ?? 'CAD', isEditing = false } = options ?? {};
        const safeCurrency = currency || 'CAD';

        try {
          return isEditing
            ? amount.toFixed(2)
            : new Intl.NumberFormat(localeInfo.languageTag, {
                style: 'currency',
                currency: safeCurrency,
              }).format(amount);
        } catch {
          return `${amount} ${safeCurrency}`;
        }
      },
      formatDate: (date: DateOnly, anchorDate: DateOnly, options?: Intl.DateTimeFormatOptions) => {
        try {
          const dateObj = typeof date === 'string' ? parseISO(date) : date;
          const anchor = typeof anchorDate === 'string' ? parseISO(anchorDate) : anchorDate;

          const diffDays = differenceInCalendarDays(anchor, dateObj);

          if (diffDays === 0) return t(tk.common.today);
          if (diffDays === 1) return t(tk.common.yesterday);
          if (diffDays === -1) return t(tk.common.tomorrow);

          if (diffDays > 1 && diffDays < 8) return t(tk.common.last7Days, { count: diffDays });
          if (diffDays < -1 && diffDays > -8) return t(tk.common.next7Days, { count: Math.abs(diffDays) });

          return new Intl.DateTimeFormat(localeInfo.languageTag, {
            dateStyle: 'medium',
            timeZone: 'UTC',
            ...options,
          }).format(dateObj);
        } catch {
          return withDate(date).toDate().toDateString();
        }
      },
      formatPercentage: (val: number) => `${val.toFixed(2)}%`,
    }),
    [localeInfo, t]
  );

  return {
    currentLocale: localeInfo.languageTag,
    currencyCode: localeInfo.currencyCode ?? 'CAD',
    ...helpers,
  };
};
