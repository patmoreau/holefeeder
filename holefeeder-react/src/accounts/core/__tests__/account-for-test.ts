import { AbstractPowerSyncDatabase } from '@powersync/common';
import { Account } from '@/accounts/core/account';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { anAccountType } from '@/shared/__tests__/enum-for-test';
import { aVariation } from '@/shared/__tests__/number-for-test';
import { anId, aString } from '@/shared/__tests__/string-for-test';
import { Variation } from '@/shared/core/variation';

export type AccountForTest = Account & {
  times: (count: number) => AccountForTest[];
  store: (db: AbstractPowerSyncDatabase) => Promise<AccountForTest>;
  remove: (db: AbstractPowerSyncDatabase) => Promise<void>;
};

const defaultAccount = (): Account => ({
  id: anId(),
  type: anAccountType(),
  name: aString(),
  favorite: false,
  openBalance: aVariation(),
  openDate: aPastDate(),
  description: aString(),
  inactive: false,
});

const times = (count: number, overrides?: Partial<Account>): AccountForTest[] => Array.from({ length: count }, () => anAccount(overrides));

const store = async (db: AbstractPowerSyncDatabase, account: AccountForTest): Promise<AccountForTest> => {
  await db.execute(
    'INSERT INTO accounts (id, type, name, favorite, open_balance, open_date, description, inactive, user_id) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)',
    [
      account.id,
      account.type,
      account.name,
      account.favorite ? 1 : 0,
      Variation.toCents(account.openBalance),
      account.openDate,
      account.description,
      account.inactive ? 1 : 0,
      anId(),
    ]
  );
  return account;
};

const remove = async (db: AbstractPowerSyncDatabase, account: Account): Promise<void> => {
  await db.execute('DELETE FROM accounts WHERE id = ?', [account.id]);
};

export const anAccount = (overrides?: Partial<Account>): AccountForTest => {
  const accountForTest: AccountForTest = {
    ...defaultAccount(),
    ...overrides,
    times: (count: number) => times(count, overrides),
    store: (db: AbstractPowerSyncDatabase) => store(db, accountForTest),
    remove: (db: AbstractPowerSyncDatabase) => remove(db, accountForTest),
  };
  return accountForTest;
};
export const toAccount = ({ times, store, remove, ...account }: AccountForTest): Account => account;
