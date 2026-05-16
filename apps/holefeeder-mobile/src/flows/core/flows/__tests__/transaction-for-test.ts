import { AbstractPowerSyncDatabase } from '@powersync/common';
import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { Transaction } from '@/flows/core/flows/transaction';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aCategoryType } from '@/shared/__tests__/enum-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
import { Money } from '@/shared/core/money';

export type TransactionForTest = Transaction & {
  times: (count: number) => TransactionForTest[];
  store: (db: AbstractPowerSyncDatabase) => Promise<TransactionForTest>;
  remove: (db: AbstractPowerSyncDatabase) => Promise<void>;
};

const defaultTransaction = (): Transaction => ({
  id: anId(),
  date: aPastDate(),
  amount: anAmount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  categoryType: aCategoryType(),
  cashflowId: undefined,
  cashflowDate: undefined,
  tags: aTagList(),
});

const times = (count: number, overrides?: Partial<Transaction>): TransactionForTest[] =>
  Array.from({ length: count }, () => aTransaction(overrides));

const store = async (db: AbstractPowerSyncDatabase, transaction: TransactionForTest): Promise<TransactionForTest> => {
  await db.execute(
    'INSERT INTO transactions (id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags, user_id) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
    [
      transaction.id,
      transaction.date,
      Money.toCents(transaction.amount),
      transaction.description,
      transaction.accountId,
      transaction.categoryId,
      transaction.cashflowId,
      transaction.cashflowDate,
      transaction.tags.join(','),
      anId(),
    ]
  );
  return transaction;
};

const remove = async (db: AbstractPowerSyncDatabase, transaction: Transaction): Promise<void> => {
  await db.execute('DELETE FROM transactions WHERE id = ?', [transaction.id]);
};

export const aTransaction = (overrides?: Partial<Transaction>): TransactionForTest => {
  const transactionForTest: TransactionForTest = {
    ...defaultTransaction(),
    ...overrides,
    times: (count: number) => times(count, overrides),
    store: async (db: AbstractPowerSyncDatabase) => store(db, transactionForTest),
    remove: (db: AbstractPowerSyncDatabase) => remove(db, transactionForTest),
  };
  return transactionForTest;
};

export const toTransaction = ({ times, store, remove, ...transaction }: TransactionForTest): Transaction => transaction;
