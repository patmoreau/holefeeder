import { router } from 'expo-router';
import { type ViewProps } from 'react-native';
import { Transaction } from '@/flows/core/flows/transaction';
import { FlowRowAmount, FlowRowIcon, FlowRowTags } from '@/flows/presentation/shared/components/FlowRow';
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
        <FlowRowIcon />
      </AppListItem.Leading>
      <AppListItem.Trailing>
        <FlowRowAmount amount={transaction.amount} date={transaction.date} />
      </AppListItem.Trailing>
      <AppText variant={'defaultSemiBold'} numberOfLines={1}>
        {transaction.description}
      </AppText>
      <AppListItem.Supporting>
        <FlowRowTags tags={transaction.tags} />
      </AppListItem.Supporting>
    </AppListItem>
  );
};
