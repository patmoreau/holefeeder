import React from 'react';
import { useTranslation } from 'react-i18next';
import { type ViewProps } from 'react-native';
import { UpcomingCard } from '@/dashboard/presentation/components/UpcomingCard';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { tk } from '@/i18n/translations';
import { EmptyState } from '@/shared/presentation/components/EmptyState';
import { AppFieldSection } from '@/shared/presentation/components/native/AppFieldSection';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListSectionTitle } from '@/shared/presentation/components/native/AppListSectionTitle';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export type UpcomingCardListProps = ViewProps & {
  upcomingFlows: UpcomingFlow[];
};

export const UpcomingCardList = ({ upcomingFlows }: UpcomingCardListProps) => {
  const { t } = useTranslation();

  if (upcomingFlows.length === 0) {
    return (
      <>
        <AppListSectionTitle title={t(tk.upcomingList.title)} />
        <EmptyState
          icon={AppIconMap.sync}
          title={t(tk.emptyStates.upcoming.title)}
          hint={t(tk.emptyStates.upcoming.hint)}
          testID="dashboard-upcoming-empty"
        />
      </>
    );
  }

  return (
    <>
      <AppListSectionTitle title={t(tk.upcomingList.title)} />
      <AppFieldSection>
        <AppListForEach>
          {upcomingFlows.map((flow) => (
            <UpcomingCard key={flow.id + flow.date} upcomingFlow={flow} />
          ))}
        </AppListForEach>
      </AppFieldSection>
    </>
  );
};
