import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { ExpenseTrendBadge } from '@/shared/presentation/components/ExpenseTrendBadge';
import { SpendingTrend } from '@/summary/core/spending-trend';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';

export const SpendingPeriodHeader = () => {
  const { t } = useTranslation();
  const summaryResult = useSummary();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  return (
    <InsightsPeriodHeader
      title={t(tk.dashboard.largeHeader.spendingTitle)}
      amount={summary.currentSpending}
      badge={SpendingTrend.isShown(summary) && <ExpenseTrendBadge variation={summary.variation} variant="amount" />}
    />
  );
};
