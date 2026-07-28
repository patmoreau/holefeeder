import { type AsyncResult, DateOnly, Id, Money } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aCreateFlowCommand } from '@/flows/core/flows/__tests__/create-flow-command-for-test';
import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { AccountVariation } from '@/flows/core/flows/account-variation';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { FlowsRepositoryErrors } from '@/flows/core/flows/flows-repository';
import { aModifyFlowCommand } from '@/flows/core/flows/modify/__tests__/modify-flow-command-for-test';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { Tag } from '@/flows/core/flows/tag';
import { TagList } from '@/flows/core/flows/tag-list';
import { Transaction } from '@/flows/core/flows/transaction';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aDateIntervalType } from '@/shared/__tests__/enum-for-test';
import { aCount } from '@/shared/__tests__/number-for-test';
import { anId } from '@/shared/__tests__/string-for-test';
import { CategoryTypes } from '@/shared/core/category-type';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { WatchQueryErrors } from '@/shared/persistence/watch-query';
import { FlowsRepositoryInPowersync } from './flows-repository-in-powersync';

describe('FlowsRepository', () => {
  let db: DatabaseForTest;
  let repository: ReturnType<typeof FlowsRepositoryInPowersync>;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    repository = FlowsRepositoryInPowersync(db);
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('create', () => {
    it('should save a purchase transaction', async () => {
      const purchase: CreateFlowCommand = aCreateFlowCommand();

      const result = await repository.create(purchase);

      expect(result.isSuccess).toBe(true);

      if (result.isFailure || result.isLoading) return;

      const dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE id = ?', [result.value]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].date).toBe(purchase.date);
      expect(dbResult[0].amount).toBe(Money.toCents(purchase.amount));
      expect(dbResult[0].description).toBe(purchase.description);
      expect(dbResult[0].account_id).toBe(purchase.accountId);
      expect(dbResult[0].category_id).toBe(purchase.categoryId);
      expect(dbResult[0].tags).toBe(TagList.toConcatenatedString(purchase.tags));
      expect(dbResult[0].cashflow_id).toBeNull();
      expect(dbResult[0].cashflow_date).toBeNull();
    });

    it('creates a cashflow on a purchase recurring transaction', async () => {
      const purchase: CreateFlowCommand = aCreateFlowCommand({
        cashflow: {
          effectiveDate: aPastDate(),
          intervalType: aDateIntervalType(),
          frequency: aCount(),
        },
      });

      const result = await repository.create(purchase);

      expect(result.isSuccess).toBe(true);

      if (result.isFailure || result.isLoading) return;

      let dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE id = ?', [result.value]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].date).toBe(purchase.date);
      expect(dbResult[0].amount).toBe(Money.toCents(purchase.amount));
      expect(dbResult[0].description).toBe(purchase.description);
      expect(dbResult[0].account_id).toBe(purchase.accountId);
      expect(dbResult[0].category_id).toBe(purchase.categoryId);
      expect(dbResult[0].tags).toBe(TagList.toConcatenatedString(purchase.tags));
      expect(dbResult[0].cashflow_id).toBeDefined();
      expect(dbResult[0].cashflow_date).toBe(purchase.cashflow?.effectiveDate);

      const cashflowId = dbResult[0].cashflow_id;
      dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM cashflows WHERE id = ?', [cashflowId]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].effective_date).toBe(purchase.cashflow?.effectiveDate);
      expect(dbResult[0].interval_type).toBe(purchase.cashflow?.intervalType);
      expect(dbResult[0].frequency).toBe(purchase.cashflow?.frequency);
      expect(dbResult[0].recurrence).toBe(1);
      expect(dbResult[0].amount).toBe(Money.toCents(purchase.amount));
      expect(dbResult[0].description).toBe(purchase.description);
      expect(dbResult[0].account_id).toBe(purchase.accountId);
      expect(dbResult[0].category_id).toBe(purchase.categoryId);
      expect(dbResult[0].tags).toBe(TagList.toConcatenatedString(purchase.tags));
      expect(dbResult[0].inactive).toBe(0);
    });
  });

  describe('modify', () => {
    it('should save a modified flow', async () => {
      const transaction = await aTransaction().store(db);
      const command = aModifyFlowCommand({ id: transaction.id });

      const result = await repository.modify(command);

      expect(result).toBeSuccessWithValue(transaction.id);

      if (result.isFailure || result.isLoading) return;

      const dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE id = ?', [result.value]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].date).toBe(command.date);
      expect(dbResult[0].amount).toBe(Money.toCents(command.amount));
      expect(dbResult[0].description).toBe(command.description);
      expect(dbResult[0].account_id).toBe(command.accountId);
      expect(dbResult[0].category_id).toBe(command.categoryId);
      expect(dbResult[0].tags).toBe(TagList.toConcatenatedString(command.tags));
      expect(dbResult[0].cashflow_id).toBe(transaction.cashflowId ?? null);
      expect(dbResult[0].cashflow_date).toBe(transaction.cashflowDate ?? null);
    });

    it('returns error when Id not found', async () => {
      const command = aModifyFlowCommand();

      const result = await repository.modify(command);

      expect(result).toBeFailureWithErrors([FlowsRepositoryErrors.modifyFlowCommandFailed]);
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      const result = await repository.modify(aModifyFlowCommand());

      expect(result).toBeFailureWithErrors([FlowsRepositoryErrors.modifyFlowCommandFailed]);
    });
  });

  describe('pay', () => {
    it('should pay a cashflow as a transaction', async () => {
      const cashflow = await aCashflow().store(db);
      const command: PayFlowCommand = PayFlowCommand.valid({
        date: cashflow.effectiveDate,
        amount: cashflow.amount,
        cashflowId: cashflow.id,
        cashflowDate: cashflow.effectiveDate,
      });

      const result = await repository.pay(command);

      expect(result.isSuccess).toBe(true);

      if (result.isFailure || result.isLoading) return;

      const dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE id = ?', [result.value]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].amount).toBe(Money.toCents(cashflow.amount));
      expect(dbResult[0].description).toBe(cashflow.description);
      expect(dbResult[0].cashflow_id).toBe(cashflow.id);
      expect(dbResult[0].cashflow_date).toBe(cashflow.effectiveDate);
      expect(dbResult[0].account_id).toBe(cashflow.accountId);
      expect(dbResult[0].category_id).toBe(cashflow.categoryId);
      expect(dbResult[0].tags).toBe(cashflow.tags.join(','));
    });
  });

  describe('deactivateUpcoming', () => {
    it('should deactivate a cashflow', async () => {
      const cashflow = await aCashflow().store(db);

      const result = await repository.deactivateUpcoming(cashflow.id);

      expect(result.isSuccess).toBe(true);

      if (result.isFailure || result.isLoading) return;

      const dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM cashflows WHERE id = ?', [cashflow.id]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].inactive).toBe(1);
    });
  });

  describe('deleteTransaction', () => {
    it('should hard delete a transaction', async () => {
      const transaction = await aTransaction().store(db);

      const result = await repository.deleteTransaction(transaction.id);

      expect(result.isSuccess).toBe(true);

      const dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE id = ?', [transaction.id]);

      expect(dbResult).toHaveLength(0);
    });

    it('succeeds when the transaction does not exist', async () => {
      const result = await repository.deleteTransaction(anId());

      expect(result.isSuccess).toBe(true);
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      const result = await repository.deleteTransaction(anId());

      expect(result).toBeFailureWithErrors([FlowsRepositoryErrors.deleteTransactionCommandFailed]);
    });
  });

  describe('transfer', () => {
    it('should save a transfer transaction', async () => {
      const transferIn = await aCategory({ type: CategoryTypes.gain, name: 'Transfer In' }).store(db);
      const transferOut = await aCategory({ type: CategoryTypes.expense, name: 'Transfer Out' }).store(db);
      const transfer: TransferFlowCommand = {
        date: DateOnly.valid('2026-01-23'),
        amount: Money.valid(50.25),
        description: 'Groceries',
        sourceAccountId: Id.newId(),
        targetAccountId: Id.newId(),
      };

      const result = await repository.transfer(transfer);

      expect(result.isSuccess).toBe(true);

      if (result.isFailure || result.isLoading) return;

      let dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE account_id = ?', [transfer.sourceAccountId]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].date).toBe('2026-01-23');
      expect(dbResult[0].amount).toBe(5025); // 50.25 * 100
      expect(dbResult[0].description).toBe('Groceries');
      expect(dbResult[0].category_id).toBe(transferOut.id);

      dbResult = await db.getAll<Record<string, unknown>>('SELECT * FROM transactions WHERE account_id = ?', [transfer.targetAccountId]);

      expect(dbResult).toHaveLength(1);
      expect(dbResult[0].date).toBe('2026-01-23');
      expect(dbResult[0].amount).toBe(5025); // 50.25 * 100
      expect(dbResult[0].description).toBe('Groceries');
      expect(dbResult[0].category_id).toBe(transferIn.id);
    });
  });

  describe('watchAccountVariation', () => {
    it('retrieves variation for the specified account', async () => {
      const account = await anAccount().store(db);
      const expense = await aCategory({ type: CategoryTypes.expense }).store(db);
      const gains = await aCategory({ type: CategoryTypes.gain }).store(db);
      await aTransaction({
        accountId: account.id,
        categoryId: expense.id,
        amount: Money.valid(123.45),
        date: DateOnly.valid('2026-02-12'),
      }).store(db);
      await aTransaction({
        accountId: account.id,
        categoryId: gains.id,
        amount: Money.valid(1000.0),
        date: DateOnly.valid('2026-02-01'),
      }).store(db);

      let result: AsyncResult<AccountVariation | undefined> | undefined;
      const unsubscribe = repository.watchAccountVariation(account.id, (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue({
        accountId: account.id,
        gains: Money.valid(1000.0),
        expenses: Money.valid(123.45),
        lastTransactionDate: DateOnly.valid('2026-02-12'),
      });

      unsubscribe();
    });

    it('returns undefined when account has no transactions', async () => {
      const account = await anAccount().store(db);

      let result: AsyncResult<AccountVariation | undefined> | undefined;
      const unsubscribe = repository.watchAccountVariation(account.id, (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue(undefined);

      unsubscribe();
    });

    it('handles database errors', async () => {
      const account = await anAccount().store(db);
      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<AccountVariation | undefined> | undefined;
      const unsubscribe = repository.watchAccountVariation(account.id, (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watchCashflowVariations', () => {
    it('retrieves cashflows never paid', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      const cashflow = await aCashflow({ categoryId: category.id, amount: Money.valid(100) }).store(db);

      let result: AsyncResult<CashflowVariation[]> | undefined;
      const unsubscribe = repository.watchCashflowVariations((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([
        {
          id: cashflow.id,
          accountId: cashflow.accountId,
          categoryType: category.type,
          amount: Money.valid(100),
          description: cashflow.description,
          effectiveDate: cashflow.effectiveDate,
          frequency: cashflow.frequency,
          intervalType: cashflow.intervalType,
          lastPaidDate: undefined,
          lastCashflowDate: undefined,
          tags: cashflow.tags,
        },
      ]);

      unsubscribe();
    });

    it('retrieves cashflows variations', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      const cashflow = await aCashflow({ categoryId: category.id, amount: Money.valid(100) }).store(db);
      const transaction = await aTransaction({
        accountId: cashflow.accountId,
        categoryId: cashflow.categoryId,
        amount: Money.valid(100),
        cashflowDate: DateOnly.valid('2025-12-31'),
        cashflowId: cashflow.id,
      }).store(db);

      let result: AsyncResult<CashflowVariation[]> | undefined;
      const unsubscribe = repository.watchCashflowVariations((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([
        {
          id: cashflow.id,
          accountId: cashflow.accountId,
          categoryType: category.type,
          amount: Money.valid(100),
          description: cashflow.description,
          effectiveDate: cashflow.effectiveDate,
          frequency: cashflow.frequency,
          intervalType: cashflow.intervalType,
          lastPaidDate: transaction.date,
          lastCashflowDate: transaction.cashflowDate,
          tags: cashflow.tags,
        },
      ]);

      unsubscribe();
    });

    it('returns empty list when no cashflow exist', async () => {
      let result: AsyncResult<CashflowVariation[]> | undefined;
      const unsubscribe = repository.watchCashflowVariations((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<CashflowVariation[]> | undefined;
      const unsubscribe = repository.watchCashflowVariations((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watchTransaction', () => {
    it('retrieves transaction', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      const transaction = await aTransaction({
        categoryId: category.id,
        amount: Money.valid(50.0),
        date: DateOnly.valid('2026-03-01'),
      }).store(db);

      let result: AsyncResult<Transaction> | undefined;
      const unsubscribe = repository.watchTransaction(transaction.id, (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue({
        id: transaction.id,
        date: transaction.date,
        amount: transaction.amount,
        description: transaction.description,
        accountId: transaction.accountId,
        categoryId: transaction.categoryId,
        categoryType: category.type,
        tags: transaction.tags,
        cashflowId: undefined,
        cashflowDate: undefined,
      });

      unsubscribe();
    });

    it('returns empty list when no transactions exist', async () => {
      let result: AsyncResult<Transaction> | undefined;
      const unsubscribe = repository.watchTransaction(anId(), (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors([WatchQueryErrors.rowNotFound]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<Transaction> | undefined;
      const unsubscribe = repository.watchTransaction(anId(), (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watchTransactions', () => {
    it('retrieves transactions', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      const transaction = await aTransaction({
        categoryId: category.id,
        amount: Money.valid(50.0),
        date: DateOnly.valid('2026-03-01'),
      }).store(db);

      let result: AsyncResult<Transaction[]> | undefined;
      const unsubscribe = repository.watchTransactions(
        (data) => {
          result = data;
        },
        undefined,
        10
      );

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([
        {
          id: transaction.id,
          date: transaction.date,
          amount: transaction.amount,
          description: transaction.description,
          accountId: transaction.accountId,
          categoryId: transaction.categoryId,
          categoryType: category.type,
          tags: transaction.tags,
          cashflowId: undefined,
          cashflowDate: undefined,
        },
      ]);

      unsubscribe();
    });

    it('orders transactions by date descending', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      const older = await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-01-01') }).store(db);
      const newer = await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-03-01') }).store(db);

      let result: AsyncResult<Transaction[]> | undefined;
      const unsubscribe = repository.watchTransactions(
        (data) => {
          result = data;
        },
        undefined,
        10
      );

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result?.isSuccess).toBe(true);
      if (result?.isSuccess) {
        expect(result.value[0].id).toBe(newer.id);
        expect(result.value[1].id).toBe(older.id);
      }

      unsubscribe();
    });

    it('respects the limit parameter', async () => {
      const category = await aCategory({ type: CategoryTypes.expense }).store(db);
      await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-01-01') }).store(db);
      await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-02-01') }).store(db);
      await aTransaction({ categoryId: category.id, date: DateOnly.valid('2026-03-01') }).store(db);

      let result: AsyncResult<Transaction[]> | undefined;
      const unsubscribe = repository.watchTransactions(
        (data) => {
          result = data;
        },
        undefined,
        2
      );

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result?.isSuccess).toBe(true);
      if (result?.isSuccess) {
        expect(result.value).toHaveLength(2);
      }

      unsubscribe();
    });

    it('returns empty list when no transactions exist', async () => {
      let result: AsyncResult<Transaction[]> | undefined;
      const unsubscribe = repository.watchTransactions(
        (data) => {
          result = data;
        },
        undefined,
        10
      );

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<Transaction[]> | undefined;
      const unsubscribe = repository.watchTransactions(
        (data) => {
          result = data;
        },
        undefined,
        10
      );

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watchTags', () => {
    it('retrieves transaction tags', async () => {
      const categoryId = Id.newId();
      await aTransaction({ categoryId, tags: TagList.valid(['groceries', 'food']) }).store(db);
      await aTransaction({ categoryId, tags: TagList.valid(['groceries', 'shopping']) }).store(db);
      await aTransaction({ categoryId, tags: TagList.valid(['food']) }).store(db);
      const validTags: Tag[] = [
        aTag({ tag: 'food', count: 2, categoryId }),
        aTag({ tag: 'groceries', count: 2, categoryId }),
        aTag({ tag: 'shopping', count: 1, categoryId }),
      ];

      let result: AsyncResult<Tag[]> | undefined;
      const unsubscribe = repository.watchTags((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue(validTags);

      unsubscribe();
    });

    it('returns not found when no tags exist', async () => {
      let result: AsyncResult<Tag[]> | undefined;
      const unsubscribe = repository.watchTags((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<Tag[]> | undefined;
      const unsubscribe = repository.watchTags((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });
});
