import { aTransaction, toTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { Transaction } from './transaction';

describe('Transaction', () => {
  const validTransaction = toTransaction(aTransaction());

  describe('valid', () => {
    it('create a valid transaction', () => {
      const result = Transaction.valid(validTransaction);

      expect(result).toEqual(validTransaction);
    });

    it('create a valid transaction with optional fields', () => {
      const transactionWithOptionals = {
        ...validTransaction,
        cashflowId: '550e8400-e29b-41d4-a716-446655440000',
        cashflowDate: '2026-02-15',
      };
      const result = Transaction.valid(transactionWithOptionals);

      expect(result).toEqual(transactionWithOptionals);
    });

    it('create a valid transaction without optional fields', () => {
      const { cashflowId, cashflowDate, ...transactionWithoutOptionals } = validTransaction;
      const result = Transaction.valid(transactionWithoutOptionals);

      expect(result).toEqual(transactionWithoutOptionals);
    });
  });
});
