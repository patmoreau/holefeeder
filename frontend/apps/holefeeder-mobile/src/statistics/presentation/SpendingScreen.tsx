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
import { SpendingHeaderSmallCard } from '@/summary/presentation/SpendingHeaderSmallCard';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { SpendingBreakdown } from './SpendingBreakdown';

export const SpendingScreen = () => {
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
          testID="spending-back-button"
        />
      </AppToolbar>
      <View style={{ flex: 1 }} testID="spending-screen">
        <AppCollapsingHeader header={header} leadingToolbar small={<SpendingHeaderSmallCard summary={summary} />}>
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
    </>
  );
};
