import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aModifyFlowCommand } from '@/flows/core/flows/modify/__tests__/modify-flow-command-for-test';
import { ModifyFlowUseCase } from '@/flows/core/flows/modify/modify-flow-use-case';

describe('ModifyFlowUseCase', () => {
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof ModifyFlowUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = ModifyFlowUseCase(repository);
  });

  it('should modify flow with valid data', async () => {
    const result = await useCase.execute(aModifyFlowCommand());

    expect(result).toBeSuccessWithValue(expect.anything());
  });
});
