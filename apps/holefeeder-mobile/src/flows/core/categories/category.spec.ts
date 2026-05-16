import { aCategory, toCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { IdErrors } from '@/shared/core/id';
import { MoneyErrors } from '@/shared/core/money';
import { Category, CategoryErrors } from './category';

describe('Category', () => {
  const validCategory = aCategory();

  it('create a valid category', () => {
    const result = Category.create(validCategory);

    expect(result).toBeSuccessWithValue(toCategory(validCategory));
  });

  it('rejects invalid name (empty)', () => {
    const result = Category.create({ ...validCategory, name: '' });
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidName]);
  });

  it('rejects invalid name (wrong type)', () => {
    const result = Category.create({ ...validCategory, name: 123 });
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidName]);
  });

  it('rejects invalid budgetAmount', () => {
    const result = Category.create({ ...validCategory, budgetAmount: -100 });
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('rejects invalid color', () => {
    const result = Category.create({ ...validCategory, color: '' });
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidColor]);
  });

  it('rejects invalid favorite (wrong type)', () => {
    const result = Category.create({ ...validCategory, favorite: 'yes' });
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidFavorite]);
  });

  it('rejects invalid system (wrong type)', () => {
    const result = Category.create({ ...validCategory, system: 'no' });
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidSystem]);
  });

  it('rejects invalid id', () => {
    const result = Category.create({ ...validCategory, id: 'invalid-id' });
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });
});
