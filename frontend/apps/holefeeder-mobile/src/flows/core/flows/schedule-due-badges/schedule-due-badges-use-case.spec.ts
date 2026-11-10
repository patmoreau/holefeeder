import { DateOnly, withDate } from '@holefeeder/shared/core';
import { BadgeSchedulerInMemory } from '@/flows/core/flows/__tests__/badge-scheduler-in-memory';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { ScheduleDueBadgesUseCase } from '@/flows/core/flows/schedule-due-badges/schedule-due-badges-use-case';

const today = DateOnly.valid('2026-11-09');
const inDays = (days: number) => withDate(today).addDays(days).toDateOnly();

describe('ScheduleDueBadgesUseCase', () => {
  let scheduler: BadgeSchedulerInMemory;
  let useCase: ReturnType<typeof ScheduleDueBadgesUseCase>;

  beforeEach(() => {
    scheduler = BadgeSchedulerInMemory();
    useCase = ScheduleDueBadgesUseCase(scheduler);
  });

  it('should schedule an empty badge when no bills are upcoming', async () => {
    const result = await useCase.execute([], today);

    expect(result).toBeSuccessWithValue(undefined);
    expect(scheduler.scheduled()).toEqual([{ today: 0, days: [] }]);
  });

  it('should count a bill due today', async () => {
    await useCase.execute([aUpcomingFlow({ date: today })], today);

    expect(scheduler.scheduled()).toEqual([{ today: 1, days: [] }]);
  });

  it('should count an overdue bill today', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(-1) })], today);

    expect(scheduler.scheduled()).toEqual([{ today: 1, days: [] }]);
  });

  it('should schedule a badge on the day a future bill is due', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(3) })], today);

    expect(scheduler.scheduled()).toEqual([{ today: 0, days: [{ date: inDays(3), count: 1 }] }]);
  });

  it('should schedule one badge for bills due on the same future day', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(3) }), aUpcomingFlow({ date: inDays(3) })], today);

    expect(scheduler.scheduled()).toEqual([{ today: 0, days: [{ date: inDays(3), count: 2 }] }]);
  });

  it('should keep counting unpaid bills on later days', async () => {
    await useCase.execute(
      [
        aUpcomingFlow({ date: inDays(-1) }),
        aUpcomingFlow({ date: today }),
        aUpcomingFlow({ date: inDays(1) }),
        aUpcomingFlow({ date: inDays(1) }),
        aUpcomingFlow({ date: inDays(5) }),
      ],
      today
    );

    expect(scheduler.scheduled()).toEqual([
      {
        today: 2,
        days: [
          { date: inDays(1), count: 4 },
          { date: inDays(5), count: 5 },
        ],
      },
    ]);
  });

  it('should order badges by date when bills are not in date order', async () => {
    await useCase.execute([aUpcomingFlow({ date: inDays(5) }), aUpcomingFlow({ date: today }), aUpcomingFlow({ date: inDays(1) })], today);

    expect(scheduler.scheduled()).toEqual([
      {
        today: 1,
        days: [
          { date: inDays(1), count: 2 },
          { date: inDays(5), count: 3 },
        ],
      },
    ]);
  });

  it('should keep only the earliest 64 days', async () => {
    const flows = Array.from({ length: 70 }, (_, index) => aUpcomingFlow({ date: inDays(70 - index) }));

    await useCase.execute(flows, today);

    const days = scheduler.scheduled()[0].days;
    expect(days).toHaveLength(64);
    expect(days[0]).toEqual({ date: inDays(1), count: 1 });
    expect(days[63]).toEqual({ date: inDays(64), count: 64 });
  });

  it('should fail when the badges cannot be scheduled', async () => {
    scheduler.isFailing(['error']);

    const result = await useCase.execute([aUpcomingFlow({ date: today })], today);

    expect(result).toBeFailureWithErrors(['error']);
  });
});
