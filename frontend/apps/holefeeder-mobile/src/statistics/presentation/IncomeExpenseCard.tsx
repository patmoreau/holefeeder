import { LocalFormatter, Money } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { DefaultSettings } from '@/shared/core/settings';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListSectionTitle } from '@/shared/presentation/components/native/AppListSectionTitle';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useSettings } from '@/shared/presentation/core/use-settings';
import { useTheme } from '@/shared/theme/core/use-theme';
import { PeriodLabel } from '@/summary/core/period-label';
import { PeriodSeries } from '@/summary/core/period-series';
import { SurplusBands } from '@/summary/core/surplus-bands';
import { usePeriodSeries } from '@/summary/presentation/core/use-period-series';
import { IncomeExpenseChartView } from './IncomeExpenseChartView';

const PERIOD_COUNT = 6;

export const IncomeExpenseCard = () => {
  const { t } = useTranslation();
  const { theme } = useTheme();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const settingsResult = useSettings();
  const seriesResult = usePeriodSeries(PERIOD_COUNT);

  if (!seriesResult.isSuccess) return null;

  const settings = settingsResult.isSuccess ? settingsResult.value : DefaultSettings;
  const series = seriesResult.value;
  const labels = PeriodLabel.forSeries(series, settings, currentLocale);
  const totals = PeriodSeries.totals(series);
  const points = series.map((period, index) => ({
    label: labels[index],
    income: Money.toCents(period.gains) / 100,
    expenses: Money.toCents(period.expenses) / 100,
  }));
  const currency = (amount: number) => LocalFormatter.currency(amount, currentLocale, currencyCode);

  const total = (label: string, amount: string, color?: string) => (
    <AppColumn alignment="start" spacing={2}>
      <AppText variant="footnote">{label}</AppText>
      <AppText variant="defaultSemiBold" textStyle={color ? { color } : undefined}>
        {amount}
      </AppText>
    </AppColumn>
  );

  return (
    <>
      <AppListSectionTitle title={t(tk.insights.chart.title)} />
      <AppFieldSection>
        <IncomeExpenseChartView
          points={points}
          incomeLabel={t(tk.insights.chart.income)}
          expensesLabel={t(tk.insights.chart.expenses)}
          incomeColor={theme.colors.chartIncome}
          expensesColor={theme.colors.chartExpenses}
          bands={SurplusBands.of(points)}
          deficitLabel={t(tk.insights.chart.deficit)}
          deficitColor={theme.colors.negative}
          currencyCode={currencyCode}
          accessibilitySummary={t(tk.insights.chart.summary, { count: PeriodSeries.surplusCount(series), total: series.length })}
          identifier="insights-income-expense-chart"
          style={{ height: 200 }}
        />
        <AppRow spacing={16} alignment="start">
          {total(t(tk.insights.chart.income), currency(totals.gains))}
          {total(t(tk.insights.chart.expenses), currency(totals.expenses))}
          {total(
            t(tk.insights.chart.net),
            currency(Math.abs(totals.net)),
            totals.net < 0 ? theme.colors.negative : totals.net > 0 ? theme.colors.positive : undefined
          )}
        </AppRow>
        <AppText variant="footnote">{t(tk.insights.chart.excludesTransfers)}</AppText>
      </AppFieldSection>
    </>
  );
};
