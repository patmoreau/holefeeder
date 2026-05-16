import { Id, IdErrors } from '@holefeeder/shared/core';
import { aUpcomingFlow } from '@/flows/core/flows/__tests__/upcoming-flow-for-test';
import { UpcomingFlow } from '@/flows/core/flows/upcoming-flow';

describe('UpcomingFlow', () => {
  describe('create', () => {
    it('succeeds with valid data', () => {
      const upcomingFlow = aUpcomingFlow();
      const result = UpcomingFlow.create(upcomingFlow);
      expect(result).toBeSuccessWithValue(upcomingFlow);
    });

    it('fails with invalid id', () => {
      const upcomingFlow = aUpcomingFlow({ id: Id.valid('') });
      const result = UpcomingFlow.create(upcomingFlow);
      expect(result).toBeFailureWithErrors([IdErrors.invalid]);
    });
  });
});
