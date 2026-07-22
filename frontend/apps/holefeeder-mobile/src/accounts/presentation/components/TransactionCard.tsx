import { Button } from '@expo/ui/swift-ui';
import { LocalFormatter, today } from '@holefeeder/shared/core';
import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { useTransactionCard } from '@/accounts/presentation/core/use-transaction-card';
import { Transaction } from '@/flows/core/flows/transaction';
import { tk } from '@/i18n/translations';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { showAlert } from '@/shared/presentation/core/show-alert';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type TransactionCardProps = {
  transaction: Transaction;
};

export const TransactionCard = ({ transaction }: TransactionCardProps) => {
  const { t } = useTranslation();
  const { currentLocale, currencyCode } = useLocaleFormatter();
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
        <AppIcon name={AppIconMap.purchase.ios} size={20} color="#FFD60A" />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <AppColumn alignment={'end'}>
          <AppText variant={'default'}>{LocalFormatter.currency(transaction.amount, currentLocale, currencyCode)}</AppText>
          <AppText variant={'footnote'}>{LocalFormatter.date(transaction.date, today(), currentLocale, t)}</AppText>
        </AppColumn>
      </AppListItem.Trailing>
      <AppSwipeActions>
        <AppText variant={'defaultSemiBold'} numberOfLines={1}>
          {transaction.description}
        </AppText>
        <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={true}>
          <Button role="destructive" label={t(tk.swipeableActions.delete)} systemImage={AppIconMap.delete.ios} onPress={handleDelete} />
        </AppSwipeActions.Actions>
      </AppSwipeActions>
      <AppListItem.Supporting>
        <AppRow spacing={2}>
          {transaction.tags.map((tag) => (
            <AppChip key={tag} selected={true} label={tag} />
          ))}
        </AppRow>
      </AppListItem.Supporting>
    </AppListItem>
  );
};
