import { aCashflow, toCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { Cashflow, CashflowErrors } from './cashflow';

describe('Cashflow', () => {
  describe('create', () => {
    it('succeeds with valid data', () => {
      const cashflow = aCashflow();
      const result = Cashflow.create(cashflow);
      expect(result).toBeSuccessWithValue(toCashflow(cashflow));
    });

    it('fails with invalid frequency', () => {
      const cashflow = aCashflow({ frequency: 0 });
      const result = Cashflow.create(cashflow);
      expect(result).toBeFailureWithErrors([CashflowErrors.invalid]);
    });

    it('fails with invalid recurrence', () => {
      const cashflow = aCashflow({ recurrence: -1 });
      const result = Cashflow.create(cashflow);
      expect(result).toBeFailureWithErrors([CashflowErrors.invalid]);
    });
  });
});
