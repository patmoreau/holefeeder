import { Money, MoneyErrors } from '@/shared/core/money';

describe('Money', () => {
  describe('create', () => {
    it('should create a Money value for positive numbers', () => {
      expect(Money.create(100)).toBeSuccessWithValue(100);
    });

    it('should create a Money value for zero', () => {
      expect(Money.create(0)).toBeSuccessWithValue(0);
    });

    it('should create a Money value for decimal numbers', () => {
      expect(Money.create(99.99)).toBeSuccessWithValue(99.99);
    });

    it('should fail for negative numbers', () => {
      const result = Money.create(-50);

      expect(result.isFailure).toBe(true);
      if (result.isFailure) {
        expect(result.errors).toContain(MoneyErrors.invalid);
      }
    });

    it('should fail for negative decimal numbers', () => {
      expect(Money.create(-0.01)).toBeFailureWithErrors([MoneyErrors.invalid]);
    });
  });

  describe('valid', () => {
    it('should create a Money value for valid numbers', () => {
      expect(Money.valid(250.5)).toBe(250.5);
    });
  });

  describe('ZERO', () => {
    it('should be zero', () => {
      expect(Money.ZERO).toBe(0);
    });

    it('should be a valid Money value', () => {
      expect(Money.create(Money.ZERO)).toBeSuccessWithValue(Money.ZERO);
    });
  });

  describe('toCents', () => {
    it('should convert Money to cents', () => {
      const money = Money.valid(123.45);
      const cents = Money.toCents(money);

      expect(cents).toBe(12345);
      expect(typeof cents).toBe('number');
    });

    it('should convert ZERO to 0 cents', () => {
      const cents = Money.toCents(Money.ZERO);
      expect(cents).toBe(0);
    });
  });

  describe('fromCents', () => {
    it('should convert cents to Money', () => {
      const money = Money.fromCents(12345);
      expect(money).toBe(123.45);
    });

    it('should convert 0 cents to ZERO', () => {
      const money = Money.fromCents(0);
      expect(money).toBe(Money.ZERO);
    });
  });

  describe('sum', () => {
    it('should sum values', () => {
      const variation = Money.sum(Money.valid(123.45), Money.valid(56.78));
      expect(variation).toBe(180.23);
    });
  });

  describe('subtract', () => {
    it('should subtract values', () => {
      const variation = Money.subtract(Money.valid(123.45), Money.valid(56.78));
      expect(variation).toBe(66.67);
    });
  });

  describe('multiply', () => {
    it('should multiply values', () => {
      const variation = Money.multiply(Money.valid(123.45), Money.valid(56.78));
      expect(variation).toBe(7009.49);
    });
  });

  describe('Equality (Strict)', () => {
    it('should return true for equal Money values', () => {
      const money1 = Money.valid(100);
      const money2 = Money.valid(100);

      expect(money1 === money2).toBe(true);
    });

    it('should return false for different Money values', () => {
      const money1 = Money.valid(100);
      const money2 = Money.valid(200);

      expect(money1 === money2).toBe(false);
    });
  });

  describe('Type safety', () => {
    it('should allow Money to be used in numeric operations', () => {
      const money = Money.valid(100);

      const doubled = money * 2;
      const sum = money + 50;

      expect(doubled).toBe(200);
      expect(sum).toBe(150);
    });
  });

  describe('Edge cases', () => {
    it('should handle very small decimal values in create by rounding', () => {
      expect(Money.create(0.001)).toBeSuccessWithValue(0);
    });

    it('should round to 2 decimal places', () => {
      expect(Money.create(10.129)).toBeSuccessWithValue(10.13);
      expect(Money.create(10.121)).toBeSuccessWithValue(10.12);
    });

    it('should handle very large values in create', () => {
      expect(Money.create(Number.MAX_SAFE_INTEGER)).toBeSuccessWithValue(Number.MAX_SAFE_INTEGER);
    });

    it('should handle floating point precision in create', () => {
      expect(Money.create(0.1 + 0.2)).toBeSuccessWithValue(0.3);
    });
  });
});
