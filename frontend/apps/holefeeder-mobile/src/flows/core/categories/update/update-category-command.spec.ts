import { FavoriteErrors, Id, IdErrors, Money, MoneyErrors } from '@holefeeder/shared/core';
import { CategoryErrors } from '@/flows/core/categories/category';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';
import { aBoolean } from '@/shared/__tests__/boolean-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { aColor, anId, aString } from '@/shared/__tests__/string-for-test';
import { CategoryTypeErrors, CategoryTypes } from '@/shared/core/category-type';

describe('UpdateCategoryCommand', () => {
  let form: Record<string, unknown>;

  beforeEach(() => {
    form = {
      id: anId(),
      name: aString(),
      type: CategoryTypes.expense,
      color: aColor(),
      budgetAmount: anAmount(),
      favorite: aBoolean(),
    };
  });

  it('succeeds with valid data', () => {
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeSuccessWithValue({
      id: Id.valid(form.id),
      name: form.name,
      type: form.type,
      color: form.color,
      budgetAmount: Money.valid(form.budgetAmount),
      favorite: form.favorite,
    });
  });

  it('returns failure if id is invalid', () => {
    form.id = '';
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if name is empty', () => {
    form.name = '';
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidName]);
  });

  it('returns failure if type is invalid', () => {
    form.type = 'not-a-type';
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryTypeErrors.invalid]);
  });

  it('returns failure if color is empty', () => {
    form.color = '';
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidColor]);
  });

  it('returns failure if budgetAmount is invalid', () => {
    form.budgetAmount = NaN;
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if favorite is invalid', () => {
    form.favorite = 'not-a-boolean';
    const result = UpdateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([FavoriteErrors.invalid]);
  });
});
