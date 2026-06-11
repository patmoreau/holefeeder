import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { UpcomingCard } from '@/dashboard/presentation/components/UpcomingCard';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';

export type UpcomingCardListProps = ViewProps & {
  upcomingFlows: UpcomingFlow[];
};

export const UpcomingCardList = ({ upcomingFlows }: UpcomingCardListProps) => {
  const { t } = useTranslation();

  return (
    <AppFieldSection title={t(tk.upcomingList.title)}>
      <AppListForEach>
        {upcomingFlows.map((flow) => (
          <UpcomingCard key={flow.id + flow.date} upcomingFlow={flow} />
        ))}
      </AppListForEach>
    </AppFieldSection>
  );
};
