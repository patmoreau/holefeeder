import { Variation } from '@/shared/core/variation';

describe('Variation', () => {
  describe('create', () => {
    it('should create a Variation value for positive numbers', () => {
      expect(Variation.create(100)).toBeSuccessWithValue(100);
    });

    it('should create a Variation value for zero', () => {
      expect(Variation.create(0)).toBeSuccessWithValue(0);
    });

    it('should create a Variation value for decimal numbers', () => {
      expect(Variation.create(99.99)).toBeSuccessWithValue(99.99);
    });

    it('should create and round to 2 decimal places', () => {
      expect(Variation.create(99.999)).toBeSuccessWithValue(100);
    });
  });

  describe('valid', () => {
    it('should valid a Variation value for valid numbers', () => {
      expect(Variation.valid(250.5)).toBe(250.5);
    });
  });

  describe('ZERO', () => {
    it('should be zero', () => {
      expect(Variation.ZERO).toBe(0);
    });

    it('should be a valid Variation value', () => {
      expect(Variation.create(Variation.ZERO)).toBeSuccessWithValue(Variation.ZERO);
    });
  });

  describe('toCents', () => {
    it('should convert Variation to cents', () => {
      const variation = Variation.valid(123.45);
      const cents = Variation.toCents(variation);

      expect(cents).toBe(12345);
      expect(typeof cents).toBe('number');
    });

    it('should convert ZERO to 0 cents', () => {
      const cents = Variation.toCents(Variation.ZERO);
      expect(cents).toBe(0);
    });
  });

  describe('fromCents', () => {
    it('should convert cents to Variation', () => {
      const variation = Variation.fromCents(12345);
      expect(variation).toBe(123.45);
    });

    it('should convert 0 cents to ZERO', () => {
      const variation = Variation.fromCents(0);
      expect(variation).toBe(Variation.ZERO);
    });
  });

  describe('sum', () => {
    it('should sum values', () => {
      const variation = Variation.sum(Variation.valid(123.45), Variation.valid(56.78));
      expect(variation).toBe(180.23);
    });
  });

  describe('subtract', () => {
    it('should subtract values', () => {
      const variation = Variation.subtract(Variation.valid(123.45), Variation.valid(56.78));
      expect(variation).toBe(66.67);
    });
  });

  describe('multiply', () => {
    it('should multiply values', () => {
      const variation = Variation.multiply(Variation.valid(123.45), Variation.valid(56.78));
      expect(variation).toBe(7009.49);
    });
  });

  describe('Equality (Strict)', () => {
    it('should return true for equal Variation values', () => {
      const variation1 = Variation.valid(100);
      const variation2 = Variation.valid(100);

      expect(variation1 === variation2).toBe(true);
    });

    it('should return false for different Variation values', () => {
      const variation1 = Variation.valid(100);
      const variation2 = Variation.valid(200);

      expect(variation1 === variation2).toBe(false);
    });
  });

  describe('Type safety', () => {
    it('should allow Variation to be used in numeric operations', () => {
      const variation = Variation.valid(100);

      const doubled = variation * 2;
      const sum = variation + 50;

      expect(doubled).toBe(200);
      expect(sum).toBe(150);
    });
  });

  describe('Edge cases', () => {
    it('should handle very small decimal values in create by rounding', () => {
      expect(Variation.create(0.001)).toBeSuccessWithValue(0);
    });

    it('should round to 2 decimal places', () => {
      expect(Variation.create(10.129)).toBeSuccessWithValue(10.13);
      expect(Variation.create(10.121)).toBeSuccessWithValue(10.12);
    });

    it('should handle very large values in create', () => {
      expect(Variation.create(Number.MAX_SAFE_INTEGER)).toBeSuccessWithValue(Number.MAX_SAFE_INTEGER);
    });

    it('should handle floating point precision in create', () => {
      expect(Variation.create(0.1 + 0.2)).toBeSuccessWithValue(0.3);
    });
  });
});
