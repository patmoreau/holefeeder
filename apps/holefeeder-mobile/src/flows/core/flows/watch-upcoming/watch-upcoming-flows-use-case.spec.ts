import { waitFor } from '@testing-library/react-native';
import { startOfMonth } from 'date-fns';
import { aCashflowVariation } from '@/flows/core/flows/__tests__/cashflow-variation-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';
import { WatchUpcomingFlowsUseCase } from '@/flows/core/flows/watch-upcoming/watch-upcoming-flows-use-case';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import type { AsyncResult } from '@/shared/core/result';
import { today, withDate } from '@/shared/core/with-date';

describe('WatchUpcomingFlowsUseCase', () => {
  const asOfDate = withDate(startOfMonth(today())).toDateOnly();
  const settings = aSettings({
    effectiveDate: asOfDate,
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchUpcomingFlowsUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = WatchUpcomingFlowsUseCase(settings, repository);
  });

  it('should return upcoming flows when repository succeeds', async () => {
    const firstCashflow = aCashflowVariation({
      effectiveDate: withDate(asOfDate).addWeeks(2).toDateOnly(),
      intervalType: DateIntervalTypes.monthly,
      frequency: 1,
      lastPaidDate: undefined,
      lastCashflowDate: undefined,
    });
    repository.addCashflowVariations(firstCashflow);
    const secondCashflow = aCashflowVariation({
      effectiveDate: withDate(asOfDate).addWeeks(3).toDateOnly(),
      intervalType: DateIntervalTypes.monthly,
      frequency: 1,
      lastPaidDate: undefined,
      lastCashflowDate: undefined,
    });
    repository.addCashflowVariations(secondCashflow);

    let result: AsyncResult<UpcomingFlow[]> | undefined;
    const unsubscribe = useCase.watch((data) => {
      result = data;
    });

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue([
      {
        id: firstCashflow.id,
        description: firstCashflow.description,
        amount: firstCashflow.amount,
        date: firstCashflow.effectiveDate,
        tags: firstCashflow.tags,
        categoryType: firstCashflow.categoryType,
      },
      {
        id: secondCashflow.id,
        description: secondCashflow.description,
        amount: secondCashflow.amount,
        date: secondCashflow.effectiveDate,
        tags: secondCashflow.tags,
        categoryType: secondCashflow.categoryType,
      },
    ]);

    unsubscribe();
  });
});
