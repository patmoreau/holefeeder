import { type AsyncResult, Id, Result, Variation } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { Account } from '@/accounts/core/account';
import { AccountsRepository, AccountsRepositoryErrors } from '@/accounts/core/accounts-repository';
import { CreateAccountCommand } from '@/accounts/core/create/create-account-command';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';
import { watchQuery } from '@/shared/persistence/watch-query';

type AccountRow = {
  id: string;
  type: string;
  name: string;
  openBalance: number;
  openDate: string;
  description: string;
  favorite: number;
  inactive: number;
};

export const AccountsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): AccountsRepository => {
  const watch = (onDataChange: (result: AsyncResult<Account[]>) => void) =>
    watchQuery<AccountRow, Account>(
      db,
      `
        SELECT id,
               type,
               name,
               open_balance as openBalance,
               open_date    as openDate,
               description,
               favorite,
               inactive
        FROM accounts
        WHERE inactive = 0
        ORDER BY favorite DESC, name
      `,
      [],
      (row) =>
        Account.valid({
          ...row,
          openBalance: Variation.fromCents(row.openBalance),
          favorite: row.favorite === 1,
          inactive: row.inactive === 1,
        }),
      onDataChange,
      'watchAccounts'
    );

  const create = async (command: CreateAccountCommand): Promise<Result<Id>> => {
    try {
      const id = Id.newId();
      await db.writeTransaction(async (tx) => {
        // user_id is left to the backend, which takes it from the token when
        // PowerSync uploads the row — same as every other insert here.
        await tx.execute(
          `INSERT INTO accounts (id, type, name, open_balance, open_date, description, favorite, inactive)
           VALUES (?, ?, ?, ?, ?, ?, ?, 0)`,
          [
            id,
            command.type,
            command.name,
            Variation.toCents(command.openBalance),
            command.openDate,
            command.description,
            command.favorite ? 1 : 0,
          ]
        );
      });
      return Result.success(id);
    } catch (error) {
      return Result.failure([String(error)]);
    }
  };

  const update = async (command: UpdateAccountCommand): Promise<Result<Id>> => {
    try {
      let rowsAffected = 0;
      await db.writeTransaction(async (tx) => {
        const result = await tx.execute(
          `UPDATE accounts
           SET type = ?, name = ?, open_balance = ?, open_date = ?, description = ?, favorite = ?, inactive = ?
           WHERE id = ?
           RETURNING *`,
          [
            command.type,
            command.name,
            Variation.toCents(command.openBalance),
            command.openDate,
            command.description,
            command.favorite ? 1 : 0,
            command.inactive ? 1 : 0,
            command.id,
          ]
        );
        rowsAffected = result.rows?.length ?? 0;
      });
      if (rowsAffected === 0) return Result.failure([AccountsRepositoryErrors.accountNotFound]);
      return Result.success(command.id);
    } catch (error) {
      return Result.failure([String(error)]);
    }
  };

  return {
    watch: watch,
    create: create,
    update: update,
  };
};
