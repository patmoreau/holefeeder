import { router } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { LatestTransactionCard } from '@/dashboard/presentation/components/LatestTransactionCard';
import { useLatestTransactions } from '@/dashboard/presentation/core/use-latest-transactions';
import { tk } from '@/i18n/translations';
import { EmptyState } from '@/shared/presentation/components/EmptyState';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListSectionTitle } from '@/shared/presentation/components/native/AppListSectionTitle';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export type LatestTransactionListProps = ViewProps & {};

export const LatestTransactionList = ({ style }: LatestTransactionListProps) => {
  const { t } = useTranslation();
  const { data } = useLatestTransactions(3);

  const transactions = data.isSuccess ? data.value : null;

  if (!transactions) {
    return null;
  }
  if (transactions.length === 0) {
    return (
      <>
        <AppListSectionTitle title={t(tk.recentTransactions.title)} />
        <EmptyState
          icon={AppIconMap.purchase}
          title={t(tk.emptyStates.transactions.title)}
          hint={t(tk.emptyStates.transactions.hint)}
          action={{ label: t(tk.emptyStates.addExpense), onPress: () => router.push('/(app)/Purchase'), testID: 'dashboard-empty-add-expense' }}
          testID="dashboard-transactions-empty"
        />
      </>
    );
  }

  return (
    <>
      <AppListSectionTitle title={t(tk.recentTransactions.title)} />
      <AppFieldSection>
        <AppListForEach>
          {transactions.map((transaction) => (
            <LatestTransactionCard key={transaction.id + transaction.date} transaction={transaction} />
          ))}
        </AppListForEach>
      </AppFieldSection>
    </>
  );
};
