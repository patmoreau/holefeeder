import { Id } from '@holefeeder/core';
import { router, Stack, useLocalSearchParams } from 'expo-router';
import React from 'react';
import { AccountHeaderLargeCard } from '@/accounts/presentation/AccountHeaderLargeCard';
import { AccountHeaderSmallCard } from '@/accounts/presentation/AccountHeaderSmallCard';
import { TransactionCard } from '@/accounts/presentation/components/TransactionCard';
import { useAccountDetail } from '@/accounts/presentation/core/use-account-detail';
import { useTransactions } from '@/accounts/presentation/core/use-transactions';
import type { CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AppView } from '@/shared/presentation/AppView';
import { CardHeaderFlashList } from '@/shared/presentation/CardHeaderFlashList';
import { AppCardDivider } from '@/shared/presentation/components/AppCardDivider';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';
import { AppIcons } from '@/shared/presentation/icons';
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
        <ErrorSheet {...errors} />
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
        <Stack.Toolbar.Button icon={AppIcons.back} onPress={() => goBack()} />
      </Stack.Toolbar>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={AppIcons.edit} onPress={onEditPress} />
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
