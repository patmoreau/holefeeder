import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { UpcomingCard } from '@/dashboard/presentation/components/UpcomingCard';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { ExpoFieldSection } from '@/shared/presentation/components/native/expo/ExpoFieldSection';

export type UpcomingCardListProps = ViewProps & {
  upcomingFlows: UpcomingFlow[];
};

export const UpcomingCardList = ({ upcomingFlows }: UpcomingCardListProps) => {
  const { t } = useTranslation();

  return (
    <ExpoFieldSection title={t(tk.upcomingList.title)}>
      <AppListForEach>
        {upcomingFlows.map((flow) => (
          <UpcomingCard key={flow.id + flow.date} upcomingFlow={flow} />
        ))}
      </AppListForEach>
    </ExpoFieldSection>
  );
};
