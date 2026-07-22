import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { DeleteTransactionUseCase } from '@/flows/core/flows/delete-transaction/delete-transaction-use-case';
import { anId } from '@/shared/__tests__/string-for-test';

describe('DeleteTransactionUseCase', () => {
  let repository: FlowsRepositoryInMemory;
  let useCase: ReturnType<typeof DeleteTransactionUseCase>;

  beforeEach(() => {
    repository = FlowsRepositoryInMemory();
    useCase = DeleteTransactionUseCase(repository);
  });

  it('should delete a transaction with a valid id', async () => {
    const id = anId();

    const result = await useCase.execute(id);

    expect(result).toBeSuccessWithValue(undefined);
    expect(repository.deletedTransactionIds()).toContain(id);
  });

  it('propagates repository failure', async () => {
    repository.isFailing(['boom']);

    const result = await useCase.execute(anId());

    expect(result).toBeFailureWithErrors(['boom']);
  });
});
