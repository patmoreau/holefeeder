import { CategoryTypes } from '@/flows/core/categories/category-type';
import { aCashflowVariation } from '@/flows/core/flows/__tests__/cashflow-variation-for-test';
import { CashflowVariation, CashflowVariationErrors } from '@/flows/core/flows/cashflow-variation';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Money } from '@/shared/core/money';

describe('Cashflow Variation', () => {
  describe('forVariations', () => {
    describe('lastPaidDate', () => {
      it('fails when never paid', () => {
        const cashflow = aCashflowVariation({ lastPaidDate: undefined, lastCashflowDate: undefined });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.lastPaidDate();

        expect(result).toBeFailureWithErrors([CashflowVariationErrors.neverPaid]);
      });

      it('succeeds when paid and returns latest date', () => {
        const cashflow = aCashflowVariation({ lastPaidDate: DateOnly.valid('2025-01-01'), lastCashflowDate: DateOnly.valid('2025-01-01') });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.lastPaidDate();

        expect(result).toBeSuccessWithValue('2025-01-01');
      });
    });

    describe('lastCashflowDate', () => {
      it('fails when never paid', () => {
        const cashflow = aCashflowVariation({ lastCashflowDate: undefined });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.lastCashflowDate();

        expect(result).toBeFailureWithErrors([CashflowVariationErrors.neverPaid]);
      });

      it('succeeds when paid and returns latest cashflow date', () => {
        const cashflow = aCashflowVariation({ lastCashflowDate: DateOnly.valid('2025-01-01') });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.lastCashflowDate();

        expect(result).toBeSuccessWithValue('2025-01-01');
      });
    });

    describe('isNotPaid', () => {
      it.each([DateOnly.valid('2025-01-01'), DateOnly.valid('2026-01-01')])(
        'is not paid when cashflow was never paid and nextDate >= effectiveDate',
        (nextDate: DateOnly) => {
          const cashflow = aCashflowVariation({
            effectiveDate: DateOnly.valid('2025-01-01'),
            lastPaidDate: undefined,
            lastCashflowDate: undefined,
          });
          const forVariation = CashflowVariation.forVariation(cashflow);

          const result = forVariation.isNotPaid(nextDate);

          expect(result).toBe(true);
        }
      );

      it('cannot be not paid when cashflow was never paid and cashflow not yet effective (nextDate < effectiveDate)', () => {
        const cashflow = aCashflowVariation({ effectiveDate: DateOnly.valid('2025-01-01') });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.isNotPaid(DateOnly.valid('2024-01-01'));

        expect(result).toBe(false);
      });

      it('is not paid when nextDate is after latest transaction date with a previous cashflow date', () => {
        const cashflow = aCashflowVariation({
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-15'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.isNotPaid(DateOnly.valid('2025-03-01'));
        expect(result).toBe(true);
      });

      it('is not paid when nextDate is after latest transaction date with a previous cashflow date', () => {
        const cashflow = aCashflowVariation({
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-15'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.isNotPaid(DateOnly.valid('2025-03-01'));
        expect(result).toBe(true);
      });

      it('is paid when nextDate is after latest transaction date with a previous cashflow date', () => {
        const cashflow = aCashflowVariation({
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-15'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.isNotPaid(DateOnly.valid('2025-02-01'));
        expect(result).toBe(false);
      });

      it('is not paid when nextDate is after latest transaction date with no previous cashflow date', () => {
        const cashflow = aCashflowVariation({
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: undefined,
          lastCashflowDate: undefined,
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.isNotPaid(DateOnly.valid('2025-03-01'));
        expect(result).toBe(true);
      });
    });

    describe('getUpcomingDates', () => {
      it('returns unpaid cashflow date', () => {
        const cashflow = aCashflowVariation({
          intervalType: DateIntervalTypes.monthly,
          frequency: 1,
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-01'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.getUpcomingDates(DateOnly.valid('2025-03-01'));
        expect(result).toStrictEqual([DateOnly.valid('2025-03-01')]);
      });

      it('returns all unpaid cashflow dates', () => {
        const cashflow = aCashflowVariation({
          intervalType: DateIntervalTypes.monthly,
          frequency: 1,
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-01'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.getUpcomingDates(DateOnly.valid('2025-06-30'));
        expect(result).toStrictEqual([
          DateOnly.valid('2025-03-01'),
          DateOnly.valid('2025-04-01'),
          DateOnly.valid('2025-05-01'),
          DateOnly.valid('2025-06-01'),
        ]);
      });
    });

    describe('calculateUpcomingVariation', () => {
      it('returns all unpaid cashflow dates', () => {
        const cashflow = aCashflowVariation({
          amount: Money.valid(10.12),
          categoryType: CategoryTypes.expense,
          intervalType: DateIntervalTypes.monthly,
          frequency: 1,
          effectiveDate: DateOnly.valid('2025-01-01'),
          lastPaidDate: DateOnly.valid('2025-02-01'),
          lastCashflowDate: DateOnly.valid('2025-02-01'),
        });
        const forVariation = CashflowVariation.forVariation(cashflow);

        const result = forVariation.calculateUpcomingVariation(DateOnly.valid('2025-06-30'));
        expect(result).toBe(-40.48);
      });
    });
  });
});
