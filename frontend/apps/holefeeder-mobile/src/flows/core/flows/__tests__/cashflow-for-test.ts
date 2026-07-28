import { Inactive, Money } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { Cashflow } from '@/flows/core/flows/cashflow';
import { TagList } from '@/flows/core/flows/tag-list';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { aCategoryType, aDateIntervalType } from '@/shared/__tests__/enum-for-test';
import { anAmount, aPositiveCount } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';

export type CashflowForTest = Cashflow & {
  times: (count: number) => CashflowForTest[];
  store: (db: AbstractPowerSyncDatabase) => Promise<CashflowForTest>;
  remove: (db: AbstractPowerSyncDatabase) => Promise<void>;
};

const defaultCashflow = (): Cashflow => ({
  id: anId(),
  effectiveDate: aPastDate(),
  amount: anAmount(),
  intervalType: aDateIntervalType(),
  frequency: aPositiveCount(),
  recurrence: aPositiveCount(),
  description: aString(),
  accountId: anId(),
  categoryId: anId(),
  categoryType: aCategoryType(),
  inactive: false as Inactive,
  tags: aTagList(),
});

const times = (count: number, overrides?: Partial<Cashflow>): CashflowForTest[] => Array.from({ length: count }, () => aCashflow(overrides));

const store = async (db: AbstractPowerSyncDatabase, cashflow: CashflowForTest): Promise<CashflowForTest> => {
  await db.execute(
    'INSERT INTO cashflows (id, effective_date, amount, interval_type, frequency, recurrence, description, account_id, category_id, inactive, tags, user_id) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)',
    [
      cashflow.id,
      cashflow.effectiveDate,
      Money.toCents(cashflow.amount),
      cashflow.intervalType,
      cashflow.frequency,
      cashflow.recurrence,
      cashflow.description,
      cashflow.accountId,
      cashflow.categoryId,
      cashflow.inactive ? 1 : 0,
      TagList.toConcatenatedString(cashflow.tags),
      anId(),
    ]
  );
  return cashflow;
};

const remove = async (db: AbstractPowerSyncDatabase, cashflow: Cashflow): Promise<void> => {
  await db.execute('DELETE FROM cashflows WHERE id = ?', [cashflow.id]);
};

export const aCashflow = (overrides?: Partial<Cashflow>): CashflowForTest => {
  const cashflow: CashflowForTest = {
    ...defaultCashflow(),
    ...overrides,
    times: (count: number) => times(count, overrides),
    store: async (db: AbstractPowerSyncDatabase) => store(db, cashflow),
    remove: (db: AbstractPowerSyncDatabase) => remove(db, cashflow),
  };
  return cashflow;
};

export const toCashflow = ({ times, store, remove, ...cashflowData }: CashflowForTest): Cashflow => cashflowData;
