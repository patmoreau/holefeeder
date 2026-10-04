import { router } from 'expo-router';
import { type ViewProps } from 'react-native';
import { FlowTitle } from '@/flows/core/flows/flow-title';
import { Transaction } from '@/flows/core/flows/transaction';
import { FlowRowAmount, FlowRowDetails, FlowRowIcon } from '@/flows/presentation/shared/components/FlowRow';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppText } from '@/shared/presentation/components/native/AppText';

export type LatestTransactionCardProps = ViewProps & {
  transaction: Transaction;
};

export const LatestTransactionCard = ({ transaction, ...props }: LatestTransactionCardProps) => {
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
        <FlowRowIcon color={transaction.categoryColor} />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <FlowRowAmount amount={transaction.amount} date={transaction.date} />
      </AppListItem.Trailing>
      <AppText variant={'defaultSemiBold'} numberOfLines={1}>
        {FlowTitle.title(transaction)}
      </AppText>
      <AppListItem.Supporting>
        <FlowRowDetails categoryName={transaction.categoryName} showsCategory={FlowTitle.showsCategory(transaction)} tags={transaction.tags} />
      </AppListItem.Supporting>
    </AppListItem>
  );
};
