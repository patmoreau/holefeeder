import { type AsyncResult, Id, Logger, Money, Result } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { AccountVariation } from '@/flows/core/flows/account-variation';
import { CashflowVariation } from '@/flows/core/flows/cashflow-variation';
import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { FlowsRepository, FlowsRepositoryErrors } from '@/flows/core/flows/flows-repository';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { Tag } from '@/flows/core/flows/tag';
import { TagList } from '@/flows/core/flows/tag-list';
import { Transaction } from '@/flows/core/flows/transaction';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { AccountVariationRow } from '@/flows/persistence/account-variation-row';
import { CashflowVariationRow } from '@/flows/persistence/cashflow-variation-row';
import { TagRow } from '@/flows/persistence/tag-row';
import { TransactionRow } from '@/flows/persistence/transaction-row';
import { watchQuery, watchSingle } from '@/shared/persistence/watch-query';

const logger = Logger.create('FlowsRepositoryInPowersync');

export const FlowsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): FlowsRepository => {
  const create = async (purchase: CreateFlowCommand): Promise<Result<Id>> => {
    const cashflow = purchase.cashflow
      ? {
          id: Id.newId(),
          effectiveDate: purchase.cashflow.effectiveDate,
          intervalType: purchase.cashflow.intervalType,
          frequency: purchase.cashflow.frequency,
          recurrence: purchase.cashflow.recurrence,
          amount: purchase.amount,
          description: purchase.description,
          accountId: purchase.accountId,
          categoryId: purchase.categoryId,
          tags: purchase.tags,
        }
      : undefined;
    const transaction = {
      id: Id.newId(),
      date: purchase.date,
      amount: purchase.amount,
      description: purchase.description,
      accountId: purchase.accountId,
      categoryId: purchase.categoryId,
      tags: purchase.tags,
    };
    try {
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `
          INSERT INTO transactions (id, date, amount, description, account_id, category_id,
                                    cashflow_id, cashflow_date, tags)
          VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
        `,
          [
            transaction.id,
            transaction.date,
            Money.toCents(transaction.amount),
            transaction.description,
            transaction.accountId,
            transaction.categoryId,
            cashflow ? cashflow.id : null,
            cashflow ? cashflow.effectiveDate : null,
            TagList.toConcatenatedString(transaction.tags),
          ]
        );
        if (cashflow) {
          await tx.execute(
            `
          INSERT INTO cashflows (id, effective_date, interval_type, frequency, recurrence, amount, description, account_id, category_id,
                                    tags, inactive)
          VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 0)
        `,
            [
              cashflow.id,
              cashflow.effectiveDate,
              cashflow.intervalType,
              cashflow.frequency,
              cashflow.recurrence,
              Money.toCents(cashflow.amount),
              cashflow.description,
              cashflow.accountId,
              cashflow.categoryId,
              TagList.toConcatenatedString(cashflow.tags),
            ]
          );
        }
      });

      return Result.success(transaction.id);
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${FlowsRepositoryErrors.createFlowCommandFailed}: `, error.message);
      }
      return Result.failure([FlowsRepositoryErrors.createFlowCommandFailed]);
    }
  };

  const modify = async (command: ModifyFlowCommand): Promise<Result<Id>> => {
    try {
      let rowsAffected = 0;
      await db.writeTransaction(async (tx) => {
        const result = await tx.execute(
          `
            UPDATE transactions
            SET date = ?, amount = ?, description = ?, account_id = ?, category_id = ?, tags = ?
            WHERE id = ?
            RETURNING *
          `,
          [
            command.date,
            Money.toCents(command.amount),
            command.description,
            command.accountId,
            command.categoryId,
            TagList.toConcatenatedString(command.tags),
            command.id,
          ]
        );
        rowsAffected = result.rows?.length ?? 0;
      });

      if (rowsAffected === 0) {
        return Result.failure([FlowsRepositoryErrors.modifyFlowCommandFailed]);
      }

      return Result.success(command.id);
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${FlowsRepositoryErrors.modifyFlowCommandFailed}: `, error.message);
      }
      return Result.failure([FlowsRepositoryErrors.modifyFlowCommandFailed]);
    }
  };

  const pay = async (command: PayFlowCommand): Promise<Result<Id>> => {
    const newId = Id.newId();

    try {
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `
            INSERT INTO transactions (id, date, amount, description, account_id, category_id, cashflow_id, cashflow_date,
                                      tags)
            SELECT ?,
                   ?,
                   ?,
                   description,
                   account_id,
                   category_id,
                   id,
                   ?,
                   tags
            FROM cashflows
            WHERE id = ?
          `,
          [newId, command.date, Money.toCents(command.amount), command.cashflowDate, command.cashflowId]
        );

        if (command.updateRecurringAmount) {
          await tx.execute(
            `
              UPDATE cashflows
              SET amount = ?
              WHERE id = ?
            `,
            [Money.toCents(command.amount), command.cashflowId]
          );
        }
      });

      return Result.success(newId);
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${FlowsRepositoryErrors.payFlowCommandFailed}: `, error.message);
      }
      return Result.failure([FlowsRepositoryErrors.payFlowCommandFailed]);
    }
  };

  const deactivateUpcoming = async (cashflowId: Id): Promise<Result<void>> => {
    try {
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `
            UPDATE cashflows
            SET inactive = 1
            WHERE id = ?
          `,
          [cashflowId]
        );
      });

      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${FlowsRepositoryErrors.payFlowCommandFailed}: `, error.message);
      }
      return Result.failure([FlowsRepositoryErrors.payFlowCommandFailed]);
    }
  };

  const deleteTransaction = async (transactionId: Id): Promise<Result<void>> => {
    try {
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `
            DELETE FROM transactions
            WHERE id = ?
          `,
          [transactionId]
        );
      });

      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error(`${FlowsRepositoryErrors.deleteTransactionCommandFailed}: `, error.message);
      }
      return Result.failure([FlowsRepositoryErrors.deleteTransactionCommandFailed]);
    }
  };

  const transfer = async (transfer: TransferFlowCommand): Promise<Result<void>> => {
    const sourceId = Id.newId();
    const targetId = Id.newId();

    try {
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `
            INSERT INTO transactions (id, date, amount, description, account_id, category_id)
            SELECT ?,
                   ?,
                   ?,
                   ?,
                   ?,
                   c.id
            FROM categories c
            WHERE name = 'Transfer Out'
          `,
          [sourceId, transfer.date, Money.toCents(transfer.amount), transfer.description, transfer.sourceAccountId]
        );

        await tx.execute(
          `
            INSERT INTO transactions (id, date, amount, description, account_id, category_id)
            SELECT ?,
                   ?,
                   ?,
                   ?,
                   ?,
                   c.id
            FROM categories c
            WHERE name = 'Transfer In'
        `,
          [targetId, transfer.date, Money.toCents(transfer.amount), transfer.description, transfer.targetAccountId]
        );
      });

      return Result.success();
    } catch (error) {
      if (error instanceof Error) {
        logger.error('Failed to save transfer:', error.message);
        return Result.failure([error.message]);
      }
      return Result.failure(['Failed to save transfer']);
    }
  };

  const watchAccountVariation = (accountId: Id, onDataChange: (result: AsyncResult<AccountVariation | undefined>) => void) =>
    watchSingle<AccountVariationRow, AccountVariation | undefined>(
      db,
      `
        SELECT
          t.account_id as accountId,
          MAX(t.date) as lastTransactionDate,
          SUM(CASE WHEN lower(c.type) = 'expense' THEN t.amount ELSE 0 END) as expenses,
          SUM(CASE WHEN lower(c.type) = 'gain' THEN t.amount ELSE 0 END) as gains
        FROM transactions t
               JOIN categories c ON t.category_id = c.id
        WHERE t.account_id = ?
        GROUP BY t.account_id
      `,
      [accountId],
      (row) =>
        AccountVariation.valid({
          ...row,
          expenses: Money.fromCents(row.expenses),
          gains: Money.fromCents(row.gains),
        }),
      onDataChange,
      () => Result.success(undefined),
      'watchAccountVariation'
    );

  const watchCashflowVariations = (onDataChange: (result: AsyncResult<CashflowVariation[]>) => void) =>
    watchQuery<CashflowVariationRow, CashflowVariation>(
      db,
      `
        WITH tx_agg AS (SELECT cashflow_id,
                               MAX(date)          AS lastPaidDate,
                               MAX(cashflow_date) AS lastCashflowDate
                        FROM transactions
                        GROUP BY cashflow_id)
        SELECT c.id             AS id,
               c.account_id     AS accountId,
               tx.lastPaidDate,
               tx.lastCashflowDate,
               c.amount,
               COALESCE(NULLIF(c.description, ''), cc.name) AS description,
               c.effective_date AS effectiveDate,
               c.frequency,
               c.interval_type  AS intervalType,
               cc.type          AS categoryType,
               c.tags           AS tags
        FROM cashflows c
               LEFT JOIN tx_agg tx ON tx.cashflow_id = c.id
               JOIN categories cc ON cc.id = c.category_id
        WHERE c.inactive = 0
      `,
      [],
      (row) => CashflowVariation.valid({ ...row, amount: Money.fromCents(row.amount), tags: TagList.fromConcatenatedString(row.tags) }),
      onDataChange,
      'watchCashflowVariations'
    );

  const watchTransaction = (transactionId: Id, onDataChange: (result: AsyncResult<Transaction>) => void) =>
    watchSingle<TransactionRow, Transaction>(
      db,
      `
        SELECT t.id,
               t.date,
               t.amount,
               t.description,
               t.account_id    AS accountId,
               t.category_id   AS categoryId,
               c.type          AS categoryType,
               t.tags,
               t.cashflow_id   AS cashflowId,
               t.cashflow_date AS cashflowDate
        FROM transactions t
               JOIN categories c ON t.category_id = c.id
        WHERE t.id = ?
      `,
      [transactionId],
      (row) =>
        Transaction.valid({
          ...row,
          amount: Money.fromCents(row.amount),
          tags: TagList.fromConcatenatedString(row.tags),
        }),
      onDataChange,
      undefined,
      'watchTransaction'
    );

  const watchTransactions = (onDataChange: (result: AsyncResult<Transaction[]>) => void, accountId?: Id, limit?: number, offset?: number) =>
    watchQuery<TransactionRow, Transaction>(
      db,
      `
        SELECT t.id,
               t.date,
               t.amount,
               COALESCE(NULLIF(t.description, ''), c.name) AS description,
               t.account_id    AS accountId,
               t.category_id   AS categoryId,
               c.type          AS categoryType,
               t.tags,
               t.cashflow_id   AS cashflowId,
               t.cashflow_date AS cashflowDate
        FROM transactions t
               JOIN categories c ON t.category_id = c.id
        WHERE (? IS NULL OR t.account_id = ?)
        ORDER BY t.date DESC, t.id DESC
        LIMIT IFNULL(?, -1)
        OFFSET IFNULL(?, 0)
      `,
      [accountId ?? null, accountId ?? null, limit ?? null, offset ?? null],
      (row) =>
        Transaction.valid({
          ...row,
          amount: Money.fromCents(row.amount),
          tags: TagList.fromConcatenatedString(row.tags),
        }),
      onDataChange,
      'watchTransactions'
    );

  const watchTransactionCount = (onDataChange: (result: AsyncResult<number>) => void, accountId?: Id) =>
    watchSingle<{ total: number }, number>(
      db,
      `SELECT COUNT(*) AS total FROM transactions WHERE  (? IS NULL OR account_id = ?)`,
      [accountId, accountId],
      (row) => row.total,
      onDataChange,
      () => Result.success(0),
      'watchTransactionCount'
    );

  const watchTags = (onDataChange: (result: AsyncResult<Tag[]>) => void) =>
    watchQuery<TagRow, Tag>(
      db,
      `
        WITH RECURSIVE split(tag, remainder, category_id) AS
                         (SELECT
                            Ltrim(Substr(tags || ',', 1, Instr(tags || ',', ',') - 1)) AS tag,
                            Substr(tags || ',', Instr(tags || ',', ',') + 1)           AS remainder,
                            category_id
                          FROM transactions
                          WHERE tags IS NOT NULL AND tags <> ''
                          UNION ALL
                          SELECT
                            Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)) AS tag,
                            Substr(remainder, Instr(remainder, ',') + 1)           AS remainder,
                            category_id
                          FROM split
                          WHERE remainder <> '')
        SELECT tag,
               category_id,
               COUNT(*) AS count
        FROM split
        WHERE tag <> ''
        GROUP BY tag, category_id;
      `,
      [],
      (row) => Tag.valid(row),
      onDataChange,
      'watchTags'
    );

  return {
    create: create,
    modify: modify,
    pay: pay,
    deactivateUpcoming: deactivateUpcoming,
    deleteTransaction: deleteTransaction,
    transfer: transfer,
    watchTags: watchTags,
    watchAccountVariation: watchAccountVariation,
    watchCashflowVariations: watchCashflowVariations,
    watchTransaction: watchTransaction,
    watchTransactions: watchTransactions,
    watchTransactionCount: watchTransactionCount,
  };
};
