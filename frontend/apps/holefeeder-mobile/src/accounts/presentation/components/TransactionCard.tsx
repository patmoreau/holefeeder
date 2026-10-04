import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { useTransactionCard } from '@/accounts/presentation/core/use-transaction-card';
import { Transaction } from '@/flows/core/flows/transaction';
import { FlowRowAmount, FlowRowIcon, FlowRowTags } from '@/flows/presentation/shared/components/FlowRow';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { showAlert } from '@/shared/presentation/core/show-alert';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type TransactionCardProps = {
  transaction: Transaction;
};

export const TransactionCard = ({ transaction }: TransactionCardProps) => {
  const { t } = useTranslation();
  const repositories = useRepositories();
  const transactionCardUseCase = useTransactionCard(repositories);
  const { showDeleteAlert } = showAlert(t);

  const handleDelete = () => {
    showDeleteAlert(transaction.description, {
      onConfirm: () => {
        transactionCardUseCase.delete(transaction);
      },
      onCancel: () => {},
    });
  };

  return (
    <AppListItem
      onPress={() =>
        router.push({
          pathname: '/(app)/flows/[id]',
          params: { id: transaction.id as string },
        })
      }
    >
      <AppListItem.Leading>
        <FlowRowIcon />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <FlowRowAmount amount={transaction.amount} date={transaction.date} />
      </AppListItem.Trailing>
      <AppSwipeActions>
        <AppText variant={'defaultSemiBold'} numberOfLines={1}>
          {transaction.description}
        </AppText>
        <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={true}>
          <AppButton variant="destructive" label={t(tk.swipeableActions.delete)} icon={AppIconMap.delete} onPress={handleDelete} />
        </AppSwipeActions.Actions>
      </AppSwipeActions>
      <AppListItem.Supporting>
        <FlowRowTags tags={transaction.tags} />
      </AppListItem.Supporting>
    </AppListItem>
  );
};
