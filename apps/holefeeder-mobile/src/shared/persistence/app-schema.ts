import { column, Schema, Table } from '@powersync/common';

export const AppSchema = new Schema({
  accounts: new Table(
    {
      type: column.text,
      name: column.text,
      favorite: column.integer,
      open_balance: column.integer,
      open_date: column.text,
      description: column.text,
      inactive: column.integer,
      user_id: column.text,
    },
    {
      indexes: {
        accounts_name: ['name'],
        accounts_inactive: ['inactive'],
      },
    }
  ),
  cashflows: new Table(
    {
      effective_date: column.text,
      amount: column.integer,
      interval_type: column.text,
      frequency: column.integer,
      recurrence: column.integer,
      description: column.text,
      account_id: column.text,
      category_id: column.text,
      inactive: column.integer,
      tags: column.text,
      user_id: column.text,
    },
    {
      indexes: {
        cashflows_account: ['account_id'],
        cashflows_category: ['category_id'],
        cashflows_inactive: ['inactive'],
      },
    }
  ),
  categories: new Table(
    {
      type: column.text,
      name: column.text,
      color: column.text,
      budget_amount: column.integer,
      favorite: column.integer,
      system: column.integer,
      user_id: column.text,
    },
    {
      indexes: {
        categories_name: ['name'],
        categories_type: ['type'],
        categories_system: ['system'],
        categories_system_type: ['system', 'type'],
      },
    }
  ),
  store_items: new Table(
    {
      code: column.text,
      data: column.text,
      user_id: column.text,
    },
    { indexes: { store_items_code_unique: ['code'] } }
  ),
  transactions: new Table(
    {
      date: column.text,
      amount: column.integer,
      description: column.text,
      account_id: column.text,
      category_id: column.text,
      cashflow_id: column.text,
      cashflow_date: column.text,
      tags: column.text,
      user_id: column.text,
    },
    {
      indexes: {
        transactions_category_date: ['category_id', 'date'],
        transactions_date: ['date'],
        // transactions_account: ['account_id'],
        // transactions_category: ['category_id'],
        // transactions_category_date_amount: ['category_id', 'date', 'amount'],
        // transactions_cashflow: ['cashflow_id'],
        // transactions_cashflow_date: ['cashflow_id', 'date', 'cashflow_date'],
        // transactions_tags: ['tags'],
        transactions_account_category_amount_date: ['account_id', 'category_id', 'amount', 'date'],
      },
    }
  ),
});
