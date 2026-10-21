import { useHeaderHeight } from 'expo-router/react-navigation';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppCollapsingHeader, useCollapsingHeader } from '@/shared/presentation/components/native/collapsing-header';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { goBack } from '@/shared/presentation/core/navigation';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { CategoryIncomeList } from './CategoryIncomeList';
import { IncomePeriodHeader } from './IncomePeriodHeader';
import { PeriodAmountSmallCard } from './PeriodAmountSmallCard';

export const IncomeScreen = () => {
  const { t } = useTranslation();
  const summaryResult = useSummary();
  const navigationHeaderHeight = useHeaderHeight();
  const header = useCollapsingHeader({ collapsedHeight: navigationHeaderHeight });

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  return (
    <>
      <AppToolbar placement="left">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.back)}
          accessibilityLabel={t(tk.common.back)}
          onPress={() => goBack()}
          testID="income-back-button"
        />
      </AppToolbar>
      <View style={{ flex: 1 }} testID="income-screen">
        <AppCollapsingHeader
          header={header}
          leadingToolbar
          small={<PeriodAmountSmallCard label={t(tk.insights.income.smallTitle)} amount={summary.totalIncome} testID="income-header-small" />}
        >
          <IncomePeriodHeader />
        </AppCollapsingHeader>

        <View style={{ flex: 1 }}>
          <AppNative style={{ flex: 1 }}>
            <AppList inset modifiers={header.listModifiers}>
              <AppColumn style={{ paddingTop: header.spacerHeight }} />
              <CategoryIncomeList />
            </AppList>
          </AppNative>
        </View>
      </View>
    </>
  );
};
