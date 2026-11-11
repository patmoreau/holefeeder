import { Money } from '@holefeeder/shared/core';
import { projectedNetFlow } from '@/dashboard/core/projected-net-flow/projected-net-flow';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { CategoryTypes } from '@/shared/core/category-type';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';

const withNetFlow = (amount: number, isOver: boolean) => ({ ...NO_SUMMARY, netFlow: { amount: amount, isOver: isOver } });

describe('projectedNetFlow', () => {
  it('should be the net flow when nothing is upcoming', () => {
    expect(projectedNetFlow(withNetFlow(120, true), [])).toBe(120);
  });

  it('should be negative when spending is over income', () => {
    expect(projectedNetFlow(withNetFlow(80, false), [])).toBe(-80);
  });

  it('should take upcoming expenses away and add upcoming gains', () => {
    const flows = [
      aUpcomingFlow({ amount: Money.valid(50), categoryType: CategoryTypes.expense }),
      aUpcomingFlow({ amount: Money.valid(200), categoryType: CategoryTypes.gain }),
    ];

    expect(projectedNetFlow(withNetFlow(100, true), flows)).toBe(250);
  });
});
