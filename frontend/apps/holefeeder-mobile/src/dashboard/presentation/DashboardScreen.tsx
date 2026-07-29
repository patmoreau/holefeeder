import { Id, Logger } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import React, { useRef } from 'react';
import { Animated, useWindowDimensions, View } from 'react-native';
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
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { borderRadius } from '@/types/theme';
import { Theme } from '@/types/theme/theme';

const logger = Logger.create('DashboardScreen');

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
  header: {
    position: 'absolute' as const,
    top: 0,
    left: 0,
    right: 0,
    zIndex: 1000,
    paddingLeft: 16,
    // justifyContent: 'flex-end' as const,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 4,
  },
  largeCardContainer: {
    position: 'absolute' as const,
    paddingTop: 20,
    bottom: 20,
    left: 20,
    right: 20,
  },
  smallCardContainer: {
    position: 'absolute' as const,
    bottom: 16,
    left: 16,
    right: 16,
    alignItems: 'flex-start' as const,
  },
});

const DashboardScreen = () => {
  const accountsQuery = useAccountDetails();
  const dashboardQuery = useSummary();
  const upcomingQuery = useUpcomingFlows();
  const { theme } = useTheme();
  const styles = useStyles(createStyles);
  const { height } = useWindowDimensions();

  const fullHeight = height / 3;

  const headerHeight = useRef(new Animated.Value(fullHeight)).current;

  const onAccountPress = (id: Id, _layout: CardLayout) =>
    router.push({
      pathname: '/(app)/accounts/[id]',
      params: { id: id as string },
    });

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
    <View style={{ flex: 1 }}>
      <Animated.View
        style={[
          styles.header,
          {
            height: headerHeight,
            backgroundColor: theme.colors.primary,
            borderRadius: borderRadius['4xl'],
          },
        ]}
      >
        <DashboardHeaderLargeCard summary={dashboard} upcomingFlows={upcomingFlows} />
      </Animated.View>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList inset>
            <AppColumn style={{ paddingTop: fullHeight - 125 }}>
              <AppReact matchContents>
                <AccountCardList accounts={accounts} onPress={onAccountPress} />
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
