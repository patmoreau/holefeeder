import { DeleteTransactionUseCase } from '@/flows/core/flows/delete-transaction/delete-transaction-use-case';
import { Transaction } from '@/flows/core/flows/transaction';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const useTransactionCard = (repositories: RepositoriesState) => {
  const deleteTransaction = (transaction: Transaction) => {
    const useCase = DeleteTransactionUseCase(repositories.flowRepository);
    return useCase.execute(transaction.id);
  };

  return {
    delete: deleteTransaction,
  };
};
