import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { AccountSummary } from '@/accounts/core/account-summary';
import { AccountsRepository } from '../accounts-repository';

export const WatchAccountDetailsUseCase = (accountsRepository: AccountsRepository) => {
  const watchDetails = (onDataChange: (result: AsyncResult<AccountSummary[]>) => void) =>
    accountsRepository.watch((result) => {
      if (result.isLoading) {
        onDataChange(Result.loading());
        return;
      }
      if (result.isFailure) {
        onDataChange(Result.failure(result.errors));
        return;
      }
      onDataChange(Result.success(result.value.map((a) => AccountSummary.valid(a))));
    });

  return {
    watchDetails: watchDetails,
  };
};
