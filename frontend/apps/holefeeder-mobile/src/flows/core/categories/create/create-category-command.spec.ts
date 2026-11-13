import { CategoryTypeErrors, CategoryTypes, FavoriteErrors, Money, MoneyErrors } from '@holefeeder/shared/core';
import { aBoolean, anAmount, aColor, aString } from '@holefeeder/shared/testkit';
import { CategoryErrors } from '@/flows/core/categories/category';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';

describe('CreateCategoryCommand', () => {
  let form: Record<string, unknown>;

  beforeEach(() => {
    form = {
      name: aString(),
      type: CategoryTypes.expense,
      color: aColor(),
      budgetAmount: anAmount(),
      favorite: aBoolean(),
    };
  });

  it('succeeds with valid data', () => {
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeSuccessWithValue({
      name: form.name,
      type: form.type,
      color: form.color,
      budgetAmount: Money.valid(form.budgetAmount),
      favorite: form.favorite,
    });
  });

  it('returns failure if name is empty', () => {
    form.name = '';
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidName]);
  });

  it('returns failure if type is invalid', () => {
    form.type = 'not-a-type';
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryTypeErrors.invalid]);
  });

  it('returns failure if color is empty', () => {
    form.color = '';
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([CategoryErrors.invalidColor]);
  });

  it('returns failure if budgetAmount is invalid', () => {
    form.budgetAmount = NaN;
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if favorite is invalid', () => {
    form.favorite = 'not-a-boolean';
    const result = CreateCategoryCommand.create(form);
    expect(result).toBeFailureWithErrors([FavoriteErrors.invalid]);
  });
});
