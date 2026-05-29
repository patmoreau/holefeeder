import { Icon } from '@expo/ui';
import { Id } from '@holefeeder/shared/core';
import { router, Stack, useLocalSearchParams } from 'expo-router';
import { AccountHeaderLargeCard } from '@/accounts/presentation/AccountHeaderLargeCard';
import { AccountHeaderSmallCard } from '@/accounts/presentation/AccountHeaderSmallCard';
import { TransactionCard } from '@/accounts/presentation/components/TransactionCard';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { useTransactions } from '@/accounts/presentation/core/use-transactions';
import type { CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AppView } from '@/shared/presentation/AppView';
import { CardHeaderFlashList } from '@/shared/presentation/CardHeaderFlashList';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppErrorSheet } from '@/shared/presentation/components/app/AppErrorSheet';
import { AppCardDivider } from '@/shared/presentation/components/AppCardDivider';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { goBack } from '@/shared/presentation/navigation';
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

  if (!account) return <LoadingIndicator />;

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
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.edit)} onPress={onEditPress} />
      </Stack.Toolbar>
      <CardHeaderFlashList
        headerBackgroundColor={theme.colors.primary}
        largeCard={<AccountHeaderLargeCard account={account} />}
        smallCard={<AccountHeaderSmallCard account={account} />}
        pagedResult={transactionsResult}
        renderItem={(item) => <TransactionCard transaction={item.item} onPress={onFlowPress} />}
        keyExtractor={(item) => item.id}
        ItemSeparatorComponent={AppCardDivider}
        ListFooterComponent={transactionsResult.loading ? <LoadingIndicator size={'small'} /> : null}
      />
    </>
  );
};
