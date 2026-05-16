import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { Settings } from '@/settings/core/settings';
import { DateInterval } from '@/shared/core/date-interval';
import { AsyncResult, Result } from '@/shared/core/result';
import { today } from '@/shared/core/with-date';
import { FlowsRepository } from '../flows-repository';

export const WatchUpcomingFlowsUseCase = (settings: Settings, repository: FlowsRepository) => {
  const watch = (onDataChange: (result: AsyncResult<UpcomingFlow[]>) => void) =>
    repository.watchCashflowVariations((result: AsyncResult<CashflowVariation[]>) => {
      if (result.isLoading || result.isFailure) {
        onDataChange(result);
        return;
      }

      const dateIntervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
      if (!dateIntervalResult.isSuccess) {
        onDataChange(dateIntervalResult);
        return;
      }

      const flows = result.value
        .flatMap((flow) => {
          const variation = CashflowVariation.forVariation(flow);
          const dates = variation.getUpcomingDates(dateIntervalResult.value.end);
          return dates.map((date) =>
            UpcomingFlow.valid({
              ...flow,
              date: date,
              amount: flow.amount,
              categoryType: flow.categoryType,
              tags: flow.tags,
            })
          );
        })
        .sort((a, b) => a.date.localeCompare(b.date));

      onDataChange(Result.success(flows));
    });
  return {
    watch: watch,
  };
};
