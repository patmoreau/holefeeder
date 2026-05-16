import { aCreateFlowCommand } from '@/flows/core/flows/__tests__/create-flow-command-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { CreateFlowUseCase } from '@/flows/core/flows/create/create-flow-use-case';

describe('CreateFlowUseCase', () => {
  let fakeRepo: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof CreateFlowUseCase>;

  beforeEach(() => {
    fakeRepo = FlowsRepositoryInMemory();
    useCase = CreateFlowUseCase(fakeRepo);
  });

  it('should create flow with valid data', async () => {
    const validFlow = aCreateFlowCommand();
    const result = await useCase.execute(validFlow);

    expect(result).toBeSuccessWithValue(expect.anything());
  });
});
