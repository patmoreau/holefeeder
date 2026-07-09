import { DateInterval, today } from '@holefeeder/shared/core';
import { parseISO } from 'date-fns';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { DefaultSettings } from '@/settings/core/settings';
import { AppText } from '@/shared/presentation/components/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';

const createStyles = () => ({
  container: {
    paddingHorizontal: spacing['3xl'],
    paddingBottom: spacing.sm,
  },
});

export const InsightsPeriodHeader = () => {
  const { t } = useTranslation();
  const settingsResult = useSettings();
  const { currentLocale } = useLocaleFormatter();
  const styles = useStyles(createStyles);

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;

  const periodLabel = useMemo(() => {
    const intervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
    if (!intervalResult.isSuccess) return null;

    const { start, end } = intervalResult.value;
    const formatter = new Intl.DateTimeFormat(currentLocale, { month: 'short', day: 'numeric', timeZone: 'UTC' });
    return `${formatter.format(parseISO(start))} – ${formatter.format(parseISO(end))}`;
  }, [settings, currentLocale]);

  if (!periodLabel) return null;

  return (
    <View style={styles.container}>
      <AppText variant="subtitle">{t(tk.insights.period, { period: periodLabel })}</AppText>
    </View>
  );
};
