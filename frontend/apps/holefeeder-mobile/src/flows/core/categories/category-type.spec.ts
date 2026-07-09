import { CategoryType, CategoryTypeErrors, CategoryTypes } from '@/flows/core/categories/category-type';

describe('CategoryType', () => {
  describe('create', () => {
    it.each(Object.values(CategoryTypes))('accepts %s for CategoryType', (value) => {
      const result = CategoryType.create(value);
      expect(result).toBeSuccessWithValue(value);
    });

    it('rejects invalid CategoryType', () => {
      const result = CategoryType.create('invalid-type');
      expect(result).toBeFailureWithErrors([CategoryTypeErrors.invalid]);
    });

    it('rejects empty CategoryType', () => {
      const result = CategoryType.create('');
      expect(result).toBeFailureWithErrors([CategoryTypeErrors.invalid]);
    });

    it('rejects wrong type CategoryType', () => {
      const result = CategoryType.create(123);
      expect(result).toBeFailureWithErrors([CategoryTypeErrors.invalid]);
    });

    it('create returns normalized value', () => {
      expect(CategoryType.create(' Expense ')).toBeSuccessWithValue(CategoryTypes.expense);
      expect(CategoryType.create(' GAIN ')).toBeSuccessWithValue(CategoryTypes.gain);
    });
  });

  describe('valid', () => {
    it('valid returns the value directly', () => {
      const value = CategoryTypes.expense;
      const result = CategoryType.valid(value);
      expect(result).toBe(value);
    });

    it('valid returns normalized value', () => {
      expect(CategoryType.valid(' Expense ')).toBe(CategoryTypes.expense);
      expect(CategoryType.valid(' GAIN ')).toBe(CategoryTypes.gain);
    });
  });

  describe('multiplier', () => {
    it.each([CategoryTypes.gain])('multiplier is 1 for %s', (type) => {
      expect(CategoryType.multiplier[type]).toBe(1);
    });

    it.each([CategoryTypes.expense])('multiplier is -1 for %s', (type) => {
      expect(CategoryType.multiplier[type]).toBe(-1);
    });
  });
});
