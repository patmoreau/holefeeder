import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { DeactivateUpcomingFlowUseCase } from '@/flows/core/flows/deactivate-upcoming/deactivate-upcoming-flow-use-case';
import { anId } from '@/shared/__tests__/string-for-test';

describe('DeactivateUpcomingFlowUseCase', () => {
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof DeactivateUpcomingFlowUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = DeactivateUpcomingFlowUseCase(repository);
  });

  it('should pay flow with valid data', async () => {
    const result = await useCase.execute(anId());

    expect(result).toBeSuccessWithValue(undefined);
  });
});
