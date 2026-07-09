import { Icon } from '@expo/ui';
import { Id, tk } from '@holefeeder/shared/core';
import { router, Stack, useLocalSearchParams } from 'expo-router';
import { t } from 'i18next';
import { AccountHeaderLargeCard } from '@/accounts/presentation/AccountHeaderLargeCard';
import { AccountHeaderSmallCard } from '@/accounts/presentation/AccountHeaderSmallCard';
import { TransactionCard } from '@/accounts/presentation/components/TransactionCard';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { useTransactions } from '@/accounts/presentation/core/use-transactions';
import type { CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AppView } from '@/shared/presentation/AppView';
import { CardHeaderFlashList } from '@/shared/presentation/CardHeaderFlashList';
import { AppCardDivider } from '@/shared/presentation/components/AppCardDivider';
import { AppErrorSheet } from '@/shared/presentation/components/native/AppErrorSheet';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { goBack } from '@/shared/presentation/core/navigation';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const AccountScreen = () => {
  const { id } = useLocalSearchParams<{ id: string }>();
  const accountId = Id.valid(id);
  const { theme } = useTheme();
  const styles = useStyles(createStyles);

  const accountQuery = useAccountDetail(accountId);
  const transactionsResult = useTransactions(accountId);

  const { data, errors } = useMultipleWatches({
    account: withDefault(() => accountQuery, null),
  });
  const onFlowPress = (id: Id, _layout: CardLayout) =>
    router.push({
      pathname: '/(app)/flows/[id]',
      params: { id: id as string },
    });

  if (errors.showError) {
    return (
      <AppView style={styles.container}>
        <AppErrorSheet {...errors} />
      </AppView>
    );
  }

  const { account } = data;

  if (!account) return <AppLoadingIndicator />;

  const onEditPress = () =>
    router.push({
      pathname: '/(app)/EditAccount',
      params: { id: accountId as string },
    });

  return (
    <>
      <Stack.Toolbar placement="left">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.back)} onPress={() => goBack()} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Menu icon={Icon.select(AppIconMap.menu)}>
          <Stack.Toolbar.MenuAction icon={Icon.select(AppIconMap.edit)} onPress={onEditPress}>
            {t(tk.accountCard.edit)}
          </Stack.Toolbar.MenuAction>
          <Stack.Toolbar.MenuAction icon={Icon.select(AppIconMap.purchase)} onPress={() => router.push('/(app)/Purchase')}>
            {t(tk.accountCard.purchase)}
          </Stack.Toolbar.MenuAction>
        </Stack.Toolbar.Menu>
      </Stack.Toolbar>
      <CardHeaderFlashList
        headerBackgroundColor={theme.colors.primary}
        largeCard={<AccountHeaderLargeCard account={account} />}
        smallCard={<AccountHeaderSmallCard account={account} />}
        pagedResult={transactionsResult}
        renderItem={(item) => <TransactionCard transaction={item.item} onPress={onFlowPress} />}
        keyExtractor={(item) => item.id}
        ItemSeparatorComponent={AppCardDivider}
        ListFooterComponent={transactionsResult.loading ? <AppLoadingIndicator size={'small'} /> : null}
      />
    </>
  );
};
