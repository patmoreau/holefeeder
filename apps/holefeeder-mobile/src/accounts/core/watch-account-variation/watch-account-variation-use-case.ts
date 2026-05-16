import { type AsyncResult, combineWatchers, DateInterval, Id, Variation } from '@holefeeder/core';
import { AccountDetail } from '@/accounts/core/account-detail';
import { AccountType } from '@/accounts/core/account-type';
import { AccountVariation } from '@/accounts/core/account-variation';
import { CategoryType, CategoryTypes } from '@/flows/core/categories/category-type';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { FlowsRepository } from '@/flows/core/flows/flows-repository';
import { Account } from '../account';
import { AccountsRepository } from '../accounts-repository';

export const WatchAccountVariationErrors = {
  notFound: 'account-not-found',
};

export const WatchAccountVariationUseCase = (
  id: Id,
  dateInterval: DateInterval,
  accountsRepository: AccountsRepository,
  flowsRepository: FlowsRepository
) => {
  const watchAccounts = (onDataChange: (result: AsyncResult<Account[]>) => void) => accountsRepository.watch((result) => onDataChange(result));

  const watchVariation = (onDataChange: (result: AsyncResult<AccountVariation | undefined>) => void) =>
    flowsRepository.watchAccountVariation(id, (result) => onDataChange(result));

  const watchCashflowVariations = (onDataChange: (result: AsyncResult<CashflowVariation[]>) => void) =>
    flowsRepository.watchCashflowVariations((result) => onDataChange(result));

  const watchDetail = (onDataChange: (result: AsyncResult<AccountDetail>) => void) =>
    combineWatchers(
      [watchAccounts, watchVariation, watchCashflowVariations],
      (accounts: Account[], accountVariation: AccountVariation | undefined, cashflowVariations: CashflowVariation[]) => {
        const account = accounts.find((a) => a.id === id);
        if (!account) {
          throw new Error(WatchAccountVariationErrors.notFound);
        }

        const lastTransactionDate = accountVariation?.lastTransactionDate ?? account.openDate;
        const expenses = Variation.valid(accountVariation?.expenses ?? 0);
        const gains = Variation.valid(accountVariation?.gains ?? 0);

        const net = Variation.sum(
          Variation.multiply(gains, CategoryType.multiplier[CategoryTypes.gain]),
          Variation.multiply(expenses, CategoryType.multiplier[CategoryTypes.expense])
        );
        const balance = Variation.sum(account.openBalance, Variation.multiply(net, AccountType.multiplier[account.type]));

        const accountCashflows = cashflowVariations.filter((cashflow) => cashflow.accountId === account.id);

        const upcomingVariation = accountCashflows.reduce(
          (acc, curr) => Variation.sum(acc, CashflowVariation.forVariation(curr).calculateUpcomingVariation(dateInterval.end)),
          Variation.ZERO
        );

        return AccountDetail.valid({
          id: account.id,
          name: account.name,
          type: account.type,
          balance: balance,
          lastTransactionDate: lastTransactionDate,
          projectedBalance: Variation.sum(balance, Variation.multiply(upcomingVariation, AccountType.multiplier[account.type])),
          upcomingVariation: upcomingVariation,
        });
      }
    )(onDataChange);

  return { watchVariation: watchDetail };
};
