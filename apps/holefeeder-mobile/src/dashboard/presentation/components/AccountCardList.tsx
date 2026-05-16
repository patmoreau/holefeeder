import { Id } from '@holefeeder/core';
import { type ViewProps } from 'react-native';
import { AccountSummary } from '@/accounts/core/account-summary';
import { AccountCard, type CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AppCardList } from '@/shared/presentation/components/AppCardList';

export type AccountCardListProps = ViewProps & {
  accounts: AccountSummary[];
  onPress?: (id: Id, layout: CardLayout) => void;
};

export const AccountCardList = ({ accounts, onPress, style }: AccountCardListProps) => {
  const cardWidth = 300;

  return (
    <AppCardList
      scrollable="horizontal"
      cardWidth={cardWidth}
      style={style}
      data={accounts}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => <AccountCard account={item} width={cardWidth} onPress={onPress} />}
    />
  );
};
