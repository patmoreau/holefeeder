import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aPayFlowCommand } from '@/flows/core/flows/__tests__/pay-flow-command-for-test';
import { PayUseCase } from '@/flows/core/flows/pay/pay-use-case';

describe('payFlowUseCase', () => {
  let fakeRepo: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof PayUseCase>;

  beforeEach(() => {
    fakeRepo = FlowsRepositoryInMemory();
    useCase = PayUseCase(fakeRepo);
  });

  it('should pay flow with valid data', async () => {
    const validFlow = aPayFlowCommand();
    const result = await useCase.execute(validFlow);

    expect(result).toBeSuccessWithValue(expect.anything());
  });
});
