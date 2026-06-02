import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { LatestTransactionCard } from '@/dashboard/presentation/components/LatestTransactionCard';
import { useLatestTransactions } from '@/dashboard/presentation/core/use-latest-transactions';
import { tk } from '@/i18n/translations';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { ExpoFieldSection } from '@/shared/presentation/components/native/expo/ExpoFieldSection';

export type LatestTransactionListProps = ViewProps & {};

export const LatestTransactionList = ({ style }: LatestTransactionListProps) => {
  const { t } = useTranslation();
  const { data } = useLatestTransactions(3);

  const transactions = data.isSuccess ? data.value : null;

  if (!transactions?.length) {
    return null;
  }

  return (
    <ExpoFieldSection title={t(tk.recentTransactions.title)}>
      <AppListForEach>
        {transactions.map((transaction) => (
          <LatestTransactionCard key={transaction.id + transaction.date} transaction={transaction} />
        ))}
      </AppListForEach>
    </ExpoFieldSection>
  );
};
