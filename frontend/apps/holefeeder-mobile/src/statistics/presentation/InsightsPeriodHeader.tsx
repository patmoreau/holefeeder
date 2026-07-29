import { DateInterval, LocalFormatter, today } from '@holefeeder/shared/core';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { DefaultSettings } from '@/shared/core/settings';
import { AppText } from '@/shared/presentation/components/AppText';
import { ExpenseTrendBadge } from '@/shared/presentation/components/ExpenseTrendBadge';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText as NativeAppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useStyles } from '@/shared/theme/core/use-styles';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { borderRadius, fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  banner: {
    backgroundColor: theme.colors.primary,
    borderBottomLeftRadius: borderRadius['4xl'],
    borderBottomRightRadius: borderRadius['4xl'],
    paddingHorizontal: spacing['2xl'],
    paddingBottom: spacing['2xl'],
    gap: spacing.xs,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 4,
  },
  period: {
    color: theme.colors.primaryText,
    opacity: 0.7,
  },
  spendingTitle: {
    color: theme.colors.primaryText,
    opacity: 0.5,
  },
  total: {
    color: theme.colors.primaryText,
    fontWeight: fontWeight.bold,
  },
});

export const InsightsPeriodHeader = () => {
  const { t, i18n } = useTranslation();
  const settingsResult = useSettings();
  const summaryResult = useSummary();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const headerHeight = useHeaderHeight();

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;
  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  const periodLabel = useMemo(() => {
    const intervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
    if (!intervalResult.isSuccess) return null;

    const { start, end } = intervalResult.value;
    return LocalFormatter.dateRange(start, end, i18n.language);
  }, [settings, i18n.language]);

  if (!periodLabel) return null;

  return (
    <View style={[styles.banner, { paddingTop: headerHeight + spacing.sm }]}>
      <AppText variant="subtitle" style={styles.period}>
        {t(tk.insights.period, { period: periodLabel })}
      </AppText>
      {summaryResult.isSuccess && (
        <>
          <AppText variant="footnote" style={styles.spendingTitle}>
            {t(tk.dashboard.largeHeader.spendingTitle)}
          </AppText>
          <AppNative matchContents>
            <AppColumn spacing={8} alignment="start">
              <NativeAppText variant="display" textStyle={styles.total}>
                {LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
              </NativeAppText>
              <ExpenseTrendBadge variation={summary.variation} variant="amount" />
            </AppColumn>
          </AppNative>
        </>
      )}
    </View>
  );
};
