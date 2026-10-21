import { DateInterval, LocalFormatter, today } from '@holefeeder/shared/core';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { ReactNode, useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { DefaultSettings } from '@/shared/core/settings';
import { AppText } from '@/shared/presentation/components/AppText';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText as NativeAppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { fontWeight, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  // The screen owns the header's background, corners and shadow now; this is only its content.
  banner: {
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.lg,
    gap: spacing.xs,
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

type InsightsPeriodHeaderProps = {
  title: string;
  amount: number;
  badge?: ReactNode;
};

export const InsightsPeriodHeader = ({ title, amount, badge }: InsightsPeriodHeaderProps) => {
  const { t, i18n } = useTranslation();
  const settingsResult = useSettings();
  const summaryResult = useSummary();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const styles = useStyles(createStyles);
  const headerHeight = useHeaderHeight();

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;

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
            {title}
          </AppText>
          <AppNative matchContents>
            <AppColumn spacing={8} alignment="start">
              <NativeAppText variant="display" textStyle={styles.total}>
                {LocalFormatter.currency(amount, currentLocale, currencyCode)}
              </NativeAppText>
              {badge}
            </AppColumn>
          </AppNative>
        </>
      )}
    </View>
  );
};
