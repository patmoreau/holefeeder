import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { IncomeTrend } from '@/summary/core/income-trend';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { IncomeTrendBadge } from './IncomeTrendBadge';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';

export const IncomePeriodHeader = () => {
  const { t } = useTranslation();
  const summaryResult = useSummary();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  return (
    <InsightsPeriodHeader
      title={t(tk.insights.income.currentTitle)}
      amount={summary.totalIncome}
      badge={IncomeTrend.isShown(summary) && <IncomeTrendBadge variation={summary.incomeVariation} />}
    />
  );
};
