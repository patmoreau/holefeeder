import React from 'react';
import { View } from 'react-native';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppCollapsingHeader, useCollapsingHeader } from '@/shared/presentation/components/native/collapsing-header';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { SpendingHeaderSmallCard } from '@/summary/presentation/SpendingHeaderSmallCard';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { SpendingBreakdown } from './SpendingBreakdown';

export default function InsightsScreen() {
  const summaryResult = useSummary();
  const header = useCollapsingHeader();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  return (
    <View style={{ flex: 1 }} testID="insights-screen">
      <AppCollapsingHeader header={header} small={<SpendingHeaderSmallCard summary={summary} />}>
        <InsightsPeriodHeader />
      </AppCollapsingHeader>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList inset modifiers={header.listModifiers}>
            <AppColumn style={{ paddingTop: header.spacerHeight }} />
            <SpendingBreakdown />
          </AppList>
        </AppNative>
      </View>
    </View>
  );
}
