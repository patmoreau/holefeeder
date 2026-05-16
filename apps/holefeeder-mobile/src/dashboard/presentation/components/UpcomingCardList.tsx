import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { UpcomingCard } from '@/dashboard/presentation/components/UpcomingCard';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppCardDivider } from '@/shared/presentation/components/AppCardDivider';
import { AppCardList } from '@/shared/presentation/components/AppCardList';

export type UpcomingCardListProps = ViewProps & {
  upcomingFlows: UpcomingFlow[];
};

export const UpcomingCardList = ({ upcomingFlows, style }: UpcomingCardListProps) => {
  const { t } = useTranslation();

  return (
    <GestureHandlerRootView style={{ flex: 1 }}>
      <AppCardList
        style={style}
        scrollable="vertical"
        header={t(tk.upcomingList.title)}
        data={upcomingFlows}
        keyExtractor={(item) => item.id + item.date}
        renderItem={({ item }) => <UpcomingCard upcomingFlow={item} />}
        ItemSeparatorComponent={() => <AppCardDivider />}
      />
    </GestureHandlerRootView>
  );
};
