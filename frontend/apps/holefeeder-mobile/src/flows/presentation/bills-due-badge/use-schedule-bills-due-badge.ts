import { Logger, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { AppState } from 'react-native';
import { BadgeScheduler } from '@/flows/core/flows/schedule-due-badges/badge-scheduler';
import { ScheduleDueBadgesUseCase } from '@/flows/core/flows/schedule-due-badges/schedule-due-badges-use-case';
import { useUpcomingFlows } from '@/flows/presentation/upcoming/use-upcoming-flows';
import { useAuth } from '@/shared/auth/core/use-auth';
import { useBillsDueBadge } from '@/shared/bills-due-badge/core/use-bills-due-badge';

const logger = Logger.create('useScheduleBillsDueBadge');

export const useScheduleBillsDueBadge = (scheduler: BadgeScheduler) => {
  const upcomingFlows = useUpcomingFlows();
  const { user } = useAuth();
  const { enabled: switchedOn, isLoading } = useBillsDueBadge();
  const enabled = switchedOn && !!user;
  const useCase = useMemo(() => ScheduleDueBadgesUseCase(scheduler), [scheduler]);
  const [activations, setActivations] = useState(0);

  useEffect(() => {
    const subscription = AppState.addEventListener('change', (appState) => {
      if (appState === 'active') setActivations((count) => count + 1);
    });
    return () => subscription.remove();
  }, []);

  useEffect(() => () => void useCase.execute([], today()), [useCase]);

  useEffect(() => {
    if (isLoading || (enabled && !upcomingFlows.isSuccess)) return;

    useCase.execute(enabled && upcomingFlows.isSuccess ? upcomingFlows.value : [], today()).then((result) => {
      if (result.isFailure) logger.error('Error scheduling the bills due badge:', result.errors);
    });
  }, [enabled, isLoading, upcomingFlows, useCase, activations]);
};
