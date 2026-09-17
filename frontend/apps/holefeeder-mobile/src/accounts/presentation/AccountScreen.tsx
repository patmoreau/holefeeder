import { Id, tk } from '@holefeeder/shared/core';
import { router, Stack, useLocalSearchParams } from 'expo-router';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { AccountHeaderLargeCard } from '@/accounts/presentation/AccountHeaderLargeCard';
import { AccountHeaderSmallCard } from '@/accounts/presentation/AccountHeaderSmallCard';
import { TransactionCardList } from '@/accounts/presentation/components/TransactionCardList';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { useAccountTransactions } from '@/accounts/presentation/core/use-account-transactions';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppCollapsingHeader, useCollapsingHeader } from '@/shared/presentation/components/native/collapsing-header';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { goBack } from '@/shared/presentation/core/navigation';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  headerContent: {
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.lg,
    gap: spacing.xs,
  },
});

export const AccountScreen = () => {
  const { t } = useTranslation();
  const { id } = useLocalSearchParams<{ id: string }>();
  const accountId = Id.valid(id);
  const styles = useStyles(createStyles);
  // A pushed screen, so the navigation header reports its own height; the safe-area inset
  // here is only the status bar and would collapse over the toolbar row.
  const navigationHeaderHeight = useHeaderHeight();
  const header = useCollapsingHeader({ collapsedHeight: navigationHeaderHeight });

  const accountQuery = useAccountDetail(accountId);
  const { transactions: transactionsResult, hasMore, loadMore } = useAccountTransactions(accountId);

  const { data, errors } = useMultipleWatches({
    account: withDefault(() => accountQuery, null),
  });

  if (errors.showError) {
    return (
      <AppNative style={{ flex: 1 }}>
        <AppErrorSheet {...errors} />
      </AppNative>
    );
  }

  const { account } = data;

  if (!account) return <AppLoadingIndicator />;

  const transactions = transactionsResult.isSuccess ? transactionsResult.value : [];

  const onEditPress = () =>
    router.push({
      pathname: '/(app)/EditAccount',
      params: { id: accountId as string },
    });

  const onPurchasePress = () =>
    router.push({
      pathname: '/(app)/Purchase',
      params: { accountId: accountId as string },
    });

  return (
    <>
      <AppToolbar placement="left">
        <AppToolbarButton icon={AppIcon.select(AppIconMap.back)} accessibilityLabel={t(tk.common.back)} onPress={() => goBack()} />
      </AppToolbar>
      {/* Stays native everywhere: Menu is a toolbar primitive with no plain-view
          equivalent, so it cannot move into the E2E header the way buttons can. */}
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Menu icon={AppIcon.select(AppIconMap.menu)}>
          <Stack.Toolbar.MenuAction icon={AppIcon.select(AppIconMap.edit)} onPress={onEditPress}>
            {t(tk.accountCard.edit)}
          </Stack.Toolbar.MenuAction>
          <Stack.Toolbar.MenuAction icon={AppIcon.select(AppIconMap.purchase)} onPress={onPurchasePress}>
            {t(tk.accountCard.purchase)}
          </Stack.Toolbar.MenuAction>
        </Stack.Toolbar.Menu>
      </Stack.Toolbar>
      <View style={styles.container}>
        <AppCollapsingHeader header={header} leadingToolbar small={<AccountHeaderSmallCard account={account} />}>
          <View style={[styles.headerContent, { paddingTop: header.collapsedHeight + spacing.sm }]}>
            <AccountHeaderLargeCard account={account} />
          </View>
        </AppCollapsingHeader>
        <AppNative style={{ flex: 1 }}>
          <AppList inset modifiers={header.listModifiers}>
            <AppColumn
              style={{ paddingTop: header.spacerHeight }}
              modifiers={[AppModifiers.listRowInsets({ top: 0, bottom: 0, leading: 0, trailing: 0 }), AppModifiers.hideListRowSeparator]}
            />
            <TransactionCardList transactions={transactions} hasMore={hasMore} onLoadMore={loadMore} />
          </AppList>
        </AppNative>
      </View>
    </>
  );
};
