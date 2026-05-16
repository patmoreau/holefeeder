import { waitFor } from '@testing-library/react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { anAccountVariation } from '@/accounts/core/__tests__/account-variation-for-test';
import { AccountsRepositoryInMemory } from '@/accounts/core/__tests__/accounts-repository-for-test';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AccountTypes } from '@/accounts/core/account-type';
import {
  WatchAccountVariationErrors,
  WatchAccountVariationUseCase,
} from '@/accounts/core/watch-account-variation/watch-account-variation-use-case';
import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aCashflowVariation } from '@/flows/core/flows/__tests__/cashflow-variation-for-test';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aDateInterval } from '@/shared/__tests__/date-interval-for-test';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Money } from '@/shared/core/money';
import { type AsyncResult } from '@/shared/core/result';
import { Variation } from '@/shared/core/variation';

describe('WatchAccountVariationUseCase', () => {
  const account = anAccount({ openBalance: Variation.valid(10), type: AccountTypes.checking });
  const otherAccount = anAccount({ openBalance: Variation.valid(5), type: AccountTypes.savings });
  const accountVariation = anAccountVariation({ accountId: account.id, expenses: Money.valid(111), gains: Money.valid(222) });
  const cashflow = aCashflowVariation({
    accountId: account.id,
    lastPaidDate: DateOnly.valid('2023-01-01'),
    lastCashflowDate: DateOnly.valid('2023-01-01'),
    amount: Money.valid(200),
    effectiveDate: DateOnly.valid('2023-01-01'),
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
    categoryType: CategoryTypes.expense,
  });
  const dateInterval = aDateInterval({ start: DateOnly.valid('2023-02-01'), end: DateOnly.valid('2023-02-28') });
  let accountsRepo: AccountsRepositoryInMemory;
  let flowsRepo: FlowsRepositoryInMemory;

  beforeEach(() => {
    accountsRepo = AccountsRepositoryInMemory();
    flowsRepo = FlowsRepositoryInMemory();
  });

  describe('watchVariation', () => {
    it('returns the matching account detail by id', async () => {
      accountsRepo.add(account, otherAccount);
      flowsRepo.addAccountVariations(accountVariation);
      flowsRepo.addCashflowVariations(cashflow);

      const useCase = WatchAccountVariationUseCase(account.id, dateInterval, accountsRepo, flowsRepo);

      let result: AsyncResult<AccountDetail> | undefined;
      const unsubscribe = useCase.watchVariation((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue(
        expect.objectContaining({
          id: account.id,
          name: account.name,
          type: account.type,
          balance: Money.valid(121),
        })
      );

      unsubscribe();
    });

    it('returns failure with notFound error when account id does not exist', async () => {
      accountsRepo.add(otherAccount);

      const useCase = WatchAccountVariationUseCase(account.id, dateInterval, accountsRepo, flowsRepo);

      let result: AsyncResult<AccountDetail> | undefined;
      const unsubscribe = useCase.watchVariation((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors([WatchAccountVariationErrors.notFound]);

      unsubscribe();
    });

    it('returns failure when accounts repository fails', async () => {
      accountsRepo.isFailing(['error']);

      const useCase = WatchAccountVariationUseCase(account.id, dateInterval, accountsRepo, flowsRepo);

      let result: AsyncResult<AccountDetail> | undefined;
      const unsubscribe = useCase.watchVariation((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['error']);

      unsubscribe();
    });

    it('returns loading when accounts repository is loading', async () => {
      accountsRepo.isLoading();

      const useCase = WatchAccountVariationUseCase(account.id, dateInterval, accountsRepo, flowsRepo);

      let result: AsyncResult<AccountDetail> | undefined;
      const unsubscribe = useCase.watchVariation((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeLoading();

      unsubscribe();
    });

    it('returns loading when flows repository is loading', async () => {
      flowsRepo.isLoading();

      const useCase = WatchAccountVariationUseCase(account.id, dateInterval, accountsRepo, flowsRepo);

      let result: AsyncResult<AccountDetail> | undefined;
      const unsubscribe = useCase.watchVariation((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeLoading();

      unsubscribe();
    });
  });
});
