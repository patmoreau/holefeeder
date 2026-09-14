import { Id, Logger } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import React from 'react';
import { View } from 'react-native';
import { type CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AccountCardList } from '@/dashboard/presentation/components/AccountCardList';
import { LatestTransactionList } from '@/dashboard/presentation/components/LatestTransactionList';
import { UpcomingCardList } from '@/dashboard/presentation/components/UpcomingCardList';
import { useAccountDetails } from '@/dashboard/presentation/core/use-account-details';
import { useUpcomingFlows } from '@/dashboard/presentation/core/use-upcoming-flows';
import { DashboardHeaderLargeCard } from '@/dashboard/presentation/DashboardHeaderLargeCard';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { AppCollapsingHeader, useCollapsingHeader } from '@/shared/presentation/components/native/collapsing-header';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { SpendingHeaderSmallCard } from '@/summary/presentation/SpendingHeaderSmallCard';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const logger = Logger.create('DashboardScreen');

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

const DashboardScreen = () => {
  const accountsQuery = useAccountDetails();
  const dashboardQuery = useSummary();
  const upcomingQuery = useUpcomingFlows();
  const styles = useStyles(createStyles);
  const header = useCollapsingHeader();

  const onAccountPress = (id: Id, _layout: CardLayout) =>
    router.push({
      pathname: '/(app)/accounts/[id]',
      params: { id: id as string },
    });

  const onAddAccountPress = () => router.push('/(app)/AddAccount');

  logger.debug('Fetching data');
  const { data, errors } = useMultipleWatches({
    accounts: withDefault(() => accountsQuery, []),
    dashboard: withDefault(() => dashboardQuery, NO_SUMMARY),
    upcomingFlows: withDefault(() => upcomingQuery, []),
  });

  if (errors.showError) {
    logger.error('Error fetching data', errors.error);
    return (
      <AppNative style={styles.container}>
        <AppErrorSheet {...errors} />
      </AppNative>
    );
  }

  const { accounts, dashboard, upcomingFlows } = data;

  return (
    <View style={{ flex: 1 }} testID="dashboard-screen">
      <AppCollapsingHeader header={header} small={<SpendingHeaderSmallCard summary={dashboard} />}>
        <View style={{ paddingTop: header.collapsedHeight, paddingHorizontal: spacing.lg }}>
          <DashboardHeaderLargeCard summary={dashboard} upcomingFlows={upcomingFlows} />
        </View>
      </AppCollapsingHeader>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList inset modifiers={header.listModifiers}>
            <AppColumn style={{ paddingTop: header.spacerHeight }}>
              <AppReact matchContents>
                <AccountCardList accounts={accounts} onPress={onAccountPress} onAddPress={onAddAccountPress} />
              </AppReact>
            </AppColumn>
            <LatestTransactionList />
            <UpcomingCardList upcomingFlows={upcomingFlows} />
          </AppList>
        </AppNative>
      </View>
    </View>
  );
};

export default DashboardScreen;
