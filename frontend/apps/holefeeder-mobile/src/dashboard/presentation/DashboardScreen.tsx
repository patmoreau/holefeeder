import { Id, Logger } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import React, { useState } from 'react';
import { useWindowDimensions, View } from 'react-native';
import Animated, { Extrapolation, interpolate, useAnimatedStyle, useSharedValue } from 'react-native-reanimated';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
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
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { SpendingHeaderSmallCard } from '@/summary/presentation/SpendingHeaderSmallCard';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const logger = Logger.create('DashboardScreen');

// The toolbar row at the bottom of the collapsed header, where its buttons sit.
const TOOLBAR_ROW_HEIGHT = 44;
const TOOLBAR_ROW_INSET = 9;

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
    overflow: 'hidden' as const,
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
    // The toolbar buttons sit slightly above the header's bottom edge, so this row is
    // raised to line up with them rather than with the edge.
    bottom: TOOLBAR_ROW_INSET,
    left: 16,
    // Clear of the toolbar buttons on the right.
    right: 72,
    // A floor rather than a fixed height, so the row still centres on the toolbar buttons at
    // small text sizes but grows instead of clipping at large ones.
    minHeight: TOOLBAR_ROW_HEIGHT,
    justifyContent: 'center' as const,
  },
});

const DashboardScreen = () => {
  const accountsQuery = useAccountDetails();
  const dashboardQuery = useSummary();
  const upcomingQuery = useUpcomingFlows();
  const { theme } = useTheme();
  const styles = useStyles(createStyles);
  const { height } = useWindowDimensions();

  const insets = useSafeAreaInsets();

  // The header is as tall as the card needs, so it follows the text size on its own. The
  // window fraction is only a starting value for the first frame, before the card has laid out.
  const [cardHeight, setCardHeight] = useState(0);
  // Capped so the largest accessibility text sizes cannot grow the header over the whole screen.
  const fullHeight = Math.min(cardHeight || height / 3, height / 2);
  // The screen sits under a transparent stack header, so the top inset is the toolbar area.
  const collapsedHeight = insets.top;

  const scrollOffset = useSharedValue(0);
  const travel = fullHeight - collapsedHeight;
  const headerStyle = useAnimatedStyle(() => ({
    height: interpolate(scrollOffset.value, [0, travel], [fullHeight, collapsedHeight], Extrapolation.CLAMP),
  }));
  // Fades in over the last part of the travel, as the large card is leaving.
  const smallCardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [travel * 0.3, travel * 0.6], [0, 1], Extrapolation.CLAMP),
  }));
  // Slides the card up with the scroll so it leaves through the top of the header.
  const cardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [0, travel * 0.5], [1, 0], Extrapolation.CLAMP),
    transform: [{ translateY: -interpolate(scrollOffset.value, [0, travel], [0, travel], Extrapolation.CLAMP) }],
  }));

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
      <Animated.View
        style={[
          styles.header,
          headerStyle,
          {
            backgroundColor: theme.colors.primary,
          },
        ]}
      >
        <Animated.View
          style={[{ paddingTop: collapsedHeight, paddingHorizontal: spacing.lg }, cardStyle]}
          onLayout={(event) => setCardHeight(event.nativeEvent.layout.height)}
        >
          <DashboardHeaderLargeCard summary={dashboard} upcomingFlows={upcomingFlows} />
        </Animated.View>
        <Animated.View style={[styles.smallCardContainer, smallCardStyle]} pointerEvents="none">
          <SpendingHeaderSmallCard summary={dashboard} />
        </Animated.View>
      </Animated.View>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList
            inset
            modifiers={[
              AppModifiers.onScrollOffsetChange((offsetY) => {
                scrollOffset.value = offsetY;
              }),
            ]}
          >
            <AppColumn style={{ paddingTop: fullHeight - 125 }}>
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
