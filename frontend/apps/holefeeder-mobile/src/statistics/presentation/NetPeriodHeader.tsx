import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { NetBalance } from '@/summary/core/net-balance';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { NetBalanceBadge } from './NetBalanceBadge';

export const NetPeriodHeader = () => {
  const { t } = useTranslation();
  const summaryResult = useSummary();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;
  const balance = NetBalance.of(summary);

  return (
    <View testID="insights-net-header">
      <InsightsPeriodHeader title={t(tk.insights.net.title)} amount={summary.netFlow.amount} badge={<NetBalanceBadge balance={balance} />} />
    </View>
  );
};
