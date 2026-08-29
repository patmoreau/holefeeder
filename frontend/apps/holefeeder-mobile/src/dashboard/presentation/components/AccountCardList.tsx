import { Id } from '@holefeeder/shared/core';
import { type ViewProps } from 'react-native';
import { AccountSummary } from '@/accounts/core/account-summary';
import { AccountCard, type CardLayout } from '@/dashboard/presentation/components/AccountCard';
import { AddAccountCard } from '@/dashboard/presentation/components/AddAccountCard';
import { AppCardList } from '@/shared/presentation/components/AppCardList';

export type AccountCardListProps = ViewProps & {
  accounts: AccountSummary[];
  onPress?: (id: Id, layout: CardLayout) => void;
  onAddPress?: () => void;
};

export const AccountCardList = ({ accounts, onPress, onAddPress, style }: AccountCardListProps) => {
  const cardWidth = 300;
  // The list takes its row height from whatever it renders, the footer included, so the
  // cards have to agree on one rather than each taking its content's height.
  const cardHeight = 190;

  return (
    <AppCardList
      scrollable="horizontal"
      cardWidth={cardWidth}
      style={style}
      data={accounts}
      keyExtractor={(item) => item.id}
      renderItem={({ item }) => <AccountCard account={item} width={cardWidth} height={cardHeight} onPress={onPress} />}
      ListFooterComponent={onAddPress ? <AddAccountCard width={cardWidth} height={cardHeight} onPress={onAddPress} /> : undefined}
    />
  );
};
