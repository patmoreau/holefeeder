import { Id, tk } from '@holefeeder/shared/core';
import { router, Stack, useLocalSearchParams } from 'expo-router';
import { useHeaderHeight } from 'expo-router/react-navigation';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { AccountHeaderLargeCard } from '@/accounts/presentation/AccountHeaderLargeCard';
import { TransactionCardList } from '@/accounts/presentation/components/TransactionCardList';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { useAccountTransactions } from '@/accounts/presentation/core/use-account-transactions';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { goBack } from '@/shared/presentation/core/navigation';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  header: {
    backgroundColor: theme.colors.primary,
    borderBottomLeftRadius: borderRadius['4xl'],
    borderBottomRightRadius: borderRadius['4xl'],
    paddingHorizontal: spacing['2xl'],
    paddingBottom: spacing['2xl'],
    gap: spacing.xs,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 4,
  },
});

export const AccountScreen = () => {
  const { t } = useTranslation();
  const { id } = useLocalSearchParams<{ id: string }>();
  const accountId = Id.valid(id);
  const styles = useStyles(createStyles);
  const headerHeight = useHeaderHeight();

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
          <Stack.Toolbar.MenuAction icon={AppIcon.select(AppIconMap.purchase)} onPress={() => router.push('/(app)/Purchase')}>
            {t(tk.accountCard.purchase)}
          </Stack.Toolbar.MenuAction>
        </Stack.Toolbar.Menu>
      </Stack.Toolbar>
      <View style={styles.container}>
        <View style={[styles.header, { paddingTop: headerHeight + spacing.sm }]}>
          <AccountHeaderLargeCard account={account} />
        </View>
        <AppNative style={{ flex: 1 }}>
          <AppList inset>
            <TransactionCardList transactions={transactions} hasMore={hasMore} onLoadMore={loadMore} />
          </AppList>
        </AppNative>
      </View>
    </>
  );
};
