import { DateInterval, today } from '@holefeeder/shared/core';
import { parseISO } from 'date-fns';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { DefaultSettings } from '@/settings/core/settings';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useSettings } from '@/shared/presentation/core/use-settings';

export const InsightsPeriodHeader = () => {
  const { t } = useTranslation();
  const settingsResult = useSettings();
  const { currentLocale } = useLocaleFormatter();

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;

  const periodLabel = useMemo(() => {
    const intervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
    if (!intervalResult.isSuccess) return null;

    const { start, end } = intervalResult.value;
    const formatter = new Intl.DateTimeFormat(currentLocale, { month: 'short', day: 'numeric', timeZone: 'UTC' });
    return `${formatter.format(parseISO(start))} – ${formatter.format(parseISO(end))}`;
  }, [settings, currentLocale]);

  if (!periodLabel) return null;

  return <AppText variant="subtitle">{t(tk.insights.period, { period: periodLabel })}</AppText>;
};
