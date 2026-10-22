import { LocalFormatter } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppFieldLink } from '@/shared/presentation/components/native/AppFieldLink';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppCollapsingHeader, useCollapsingHeader } from '@/shared/presentation/components/native/collapsing-header';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { IncomeExpenseCard } from './IncomeExpenseCard';
import { NetPeriodHeader } from './NetPeriodHeader';
import { PeriodAmountSmallCard } from './PeriodAmountSmallCard';

export default function InsightsScreen() {
  const { t } = useTranslation();
  const summaryResult = useSummary();
  const { currentLocale, currencyCode } = useLocaleFormatter();
  const header = useCollapsingHeader();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  return (
    <View style={{ flex: 1 }} testID="insights-screen">
      <AppCollapsingHeader
        header={header}
        small={<PeriodAmountSmallCard label={t(tk.insights.net.smallTitle)} amount={summary.netFlow.amount} testID="net-header-small" />}
      >
        <NetPeriodHeader />
      </AppCollapsingHeader>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList inset modifiers={header.listModifiers}>
            <AppColumn style={{ paddingTop: header.spacerHeight }} />
            <IncomeExpenseCard />
            <AppFieldLink
              label={t(tk.insights.income.link)}
              value={LocalFormatter.currency(summary.totalIncome, currentLocale, currencyCode)}
              icon={AppIconMap.income}
              onPress={() => router.push('/(app)/insights/income')}
              testID="insights-income-link"
            />
            <AppFieldLink
              label={t(tk.insights.spending.link)}
              value={LocalFormatter.currency(summary.currentSpending, currentLocale, currencyCode)}
              icon={AppIconMap.purchase}
              onPress={() => router.push('/(app)/insights/spending')}
              testID="insights-spending-link"
            />
          </AppList>
        </AppNative>
      </View>
    </View>
  );
}
