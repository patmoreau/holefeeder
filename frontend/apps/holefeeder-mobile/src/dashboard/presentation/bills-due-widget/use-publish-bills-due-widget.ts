import { Logger, Result, today } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { AppState } from 'react-native';
import { BillsDueWidgetPublisher } from '@/dashboard/core/publish-bills-due-widget/bills-due-widget-publisher';
import { PublishBillsDueWidgetUseCase } from '@/dashboard/core/publish-bills-due-widget/publish-bills-due-widget-use-case';
import { useUpcomingFlows } from '@/flows/presentation/upcoming/use-upcoming-flows';
import { useAuth } from '@/shared/auth/core/use-auth';

const logger = Logger.create('usePublishBillsDueWidget');

const logFailure = (result: Result<void>) => {
  if (result.isFailure) logger.error('Error publishing the bills due widget:', result.errors);
};

export const usePublishBillsDueWidget = (publisher: BillsDueWidgetPublisher) => {
  const upcomingFlows = useUpcomingFlows();
  const { user } = useAuth();
  const useCase = useMemo(() => PublishBillsDueWidgetUseCase(publisher), [publisher]);
  const [activations, setActivations] = useState(0);

  useEffect(() => {
    const subscription = AppState.addEventListener('change', (appState) => {
      if (appState === 'active') setActivations((count) => count + 1);
    });
    return () => subscription.remove();
  }, []);

  useEffect(() => () => void useCase.clear().then(logFailure), [useCase]);

  useEffect(() => {
    if (!user) {
      useCase.clear().then(logFailure);
      return;
    }
    if (!upcomingFlows.isSuccess) return;

    useCase.execute(upcomingFlows.value, today()).then(logFailure);
  }, [user, upcomingFlows, useCase, activations]);
};
