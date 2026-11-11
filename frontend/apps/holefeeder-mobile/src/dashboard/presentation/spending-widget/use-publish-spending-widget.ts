import { Logger, Result } from '@holefeeder/shared/core';
import { useEffect, useMemo, useState } from 'react';
import { AppState } from 'react-native';
import { PublishSpendingWidgetUseCase } from '@/dashboard/core/publish-spending-widget/publish-spending-widget-use-case';
import { SpendingWidgetPublisher } from '@/dashboard/core/publish-spending-widget/spending-widget-publisher';
import { useUpcomingFlows } from '@/flows/presentation/upcoming/use-upcoming-flows';
import { useAuth } from '@/shared/auth/core/use-auth';
import { useSummary } from '@/summary/presentation/core/use-summary';

const logger = Logger.create('usePublishSpendingWidget');

const logFailure = (result: Result<void>) => {
  if (result.isFailure) logger.error('Error publishing the spending widget:', result.errors);
};

export const usePublishSpendingWidget = (publisher: SpendingWidgetPublisher) => {
  const upcomingFlows = useUpcomingFlows();
  const summary = useSummary();
  const { user } = useAuth();
  const useCase = useMemo(() => PublishSpendingWidgetUseCase(publisher), [publisher]);
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
    if (!upcomingFlows.isSuccess || !summary.isSuccess) return;

    useCase.execute(summary.value, upcomingFlows.value).then(logFailure);
  }, [user, upcomingFlows, summary, useCase, activations]);
};
