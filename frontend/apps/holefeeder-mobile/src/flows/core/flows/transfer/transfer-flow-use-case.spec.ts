import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTransferFlowCommand } from '@/flows/core/flows/transfer/__tests__/transfer-flow-command-for-test';
import { TransferFlowUseCase } from '@/flows/core/flows/transfer/transfer-flow-use-case';

describe('TransferFlowUseCase', () => {
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof TransferFlowUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = TransferFlowUseCase(repository);
  });

  it('should transfer flow with valid data', async () => {
    const result = await useCase.execute(aTransferFlowCommand());

    expect(result).toBeSuccessWithValue(undefined);
  });
});
