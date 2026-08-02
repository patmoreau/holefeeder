import { Money } from '@holefeeder/shared/core';
import { useUpcomingFlow } from '@/dashboard/presentation/core/use-pay-form';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { aRepositoriesState } from '@/shared/repositories/__tests__/repositories-state-for-test';

describe('useUpcomingFlow', () => {
  let flowRepository: FlowsRepositoryInMemory;
  let hook: ReturnType<typeof useUpcomingFlow>;

  beforeEach(() => {
    flowRepository = FlowsRepositoryInMemory();
    hook = useUpcomingFlow(aRepositoriesState({ flowRepository }));
  });

  describe('pay', () => {
    it('should pay the flow amount for its id and date', async () => {
      const upcomingFlow = aUpcomingFlow();

      await hook.pay(upcomingFlow);

      expect(flowRepository.paidCommands()).toEqual([
        {
          date: upcomingFlow.date,
          amount: upcomingFlow.amount,
          cashflowId: upcomingFlow.id,
          cashflowDate: upcomingFlow.date,
          updateRecurringAmount: false,
        },
      ]);
    });

    it('should return the repository success result', async () => {
      const result = await hook.pay(aUpcomingFlow());

      expect(result).toBeSuccessWithValue(expect.anything());
    });

    it('should propagate a repository failure', async () => {
      flowRepository.isFailing(['pay-flow-command-failed']);

      const result = await hook.pay(aUpcomingFlow());

      expect(result).toBeFailureWithErrors(['pay-flow-command-failed']);
    });
  });

  describe('clear', () => {
    it('should pay a zero amount while keeping the flow id and date', async () => {
      const upcomingFlow = aUpcomingFlow();

      await hook.clear(upcomingFlow);

      expect(flowRepository.paidCommands()).toEqual([
        {
          date: upcomingFlow.date,
          amount: Money.ZERO,
          cashflowId: upcomingFlow.id,
          cashflowDate: upcomingFlow.date,
          updateRecurringAmount: false,
        },
      ]);
    });

    it('should return the repository success result', async () => {
      const result = await hook.clear(aUpcomingFlow());

      expect(result).toBeSuccessWithValue(expect.anything());
    });
  });

  describe('delete', () => {
    it('should deactivate the flow id', async () => {
      const upcomingFlow = aUpcomingFlow();

      await hook.delete(upcomingFlow);

      expect(flowRepository.deactivatedCashflowIds()).toEqual([upcomingFlow.id]);
    });

    it('should return the repository success result', async () => {
      const result = await hook.delete(aUpcomingFlow());

      expect(result).toBeSuccessWithValue(undefined);
    });

    it('should propagate a repository failure', async () => {
      flowRepository.isFailing(['deactivate-failed']);

      const result = await hook.delete(aUpcomingFlow());

      expect(result).toBeFailureWithErrors(['deactivate-failed']);
    });
  });
});
