import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { LatestTransactionCard } from '@/dashboard/presentation/components/LatestTransactionCard';
import { useLatestTransactions } from '@/dashboard/presentation/core/use-latest-transactions';
import { tk } from '@/i18n/translations';
import { AppCardDivider } from '@/shared/presentation/components/AppCardDivider';
import { AppCardList } from '@/shared/presentation/components/AppCardList';

export type LatestTransactionListProps = ViewProps & {};

export const LatestTransactionList = ({ style }: LatestTransactionListProps) => {
  const { t } = useTranslation();
  const { data } = useLatestTransactions(3);

  const transactions = data.isSuccess ? data.value : null;

  if (!transactions?.length) {
    return null;
  }

  return (
    <AppCardList
      scrollable="vertical"
      style={style}
      header={t(tk.recentTransactions.title)}
      data={transactions}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => <LatestTransactionCard transaction={item} />}
      ItemSeparatorComponent={() => <AppCardDivider />}
    />
  );
};
