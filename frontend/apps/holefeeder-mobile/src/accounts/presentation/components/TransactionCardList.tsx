import { ProgressView } from '@expo/ui/swift-ui';
import { onAppear } from '@expo/ui/swift-ui/modifiers';
import { useTranslation } from 'react-i18next';
import { TransactionCard } from '@/accounts/presentation/components/TransactionCard';
import { Transaction } from '@/flows/core/flows/transaction';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';

export type TransactionCardListProps = {
  transactions: Transaction[];
  hasMore?: boolean;
  onLoadMore?: () => void;
};

export const TransactionCardList = ({ transactions, hasMore = false, onLoadMore }: TransactionCardListProps) => {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.transactionList.title)}>
      <AppListForEach>
        {transactions.map((transaction) => (
          <TransactionCard key={transaction.id} transaction={transaction} />
        ))}
      </AppListForEach>
      {hasMore && (
        // Sentinel row: when it scrolls into view the list loads the next page,
        // giving infinite scroll. Keyed by count so it re-fires each page.
        <ProgressView key={transactions.length} modifiers={[onAppear(() => onLoadMore?.())]} />
      )}
    </AppFieldSection>
  );
};
