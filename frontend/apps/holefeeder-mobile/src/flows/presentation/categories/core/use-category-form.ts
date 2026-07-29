import { Result } from '@holefeeder/shared/core';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';
import { CreateCategoryUseCase } from '@/flows/core/categories/create/create-category-use-case';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';
import { UpdateCategoryUseCase } from '@/flows/core/categories/update/update-category-use-case';
import { CategoryFormData } from '@/flows/presentation/categories/core/category-form-data';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

const CategoryFormError = {
  nameRequired: 'nameRequired',
} as const;

export type CategoryFormError = (typeof CategoryFormError)[keyof typeof CategoryFormError];

export const validateCategoryForm: ValidationFunction<CategoryFormData, CategoryFormError> = (formData) => {
  const errors: Partial<Record<keyof CategoryFormData, CategoryFormError>> = {};
  if (!formData.name.trim()) {
    errors.name = CategoryFormError.nameRequired;
  }
  return errors;
};

const saveCategory = async (repositories: RepositoriesState, formData: CategoryFormData): Promise<Result<unknown>> => {
  if (formData.id === null) {
    const commandResult = CreateCategoryCommand.create({
      name: formData.name,
      type: formData.type,
      color: formData.color,
      budgetAmount: formData.budgetAmount,
      favorite: formData.favorite,
    });
    if (commandResult.isFailure) return commandResult;

    return await CreateCategoryUseCase(repositories.categoryRepository).execute(commandResult.value);
  }

  const commandResult = UpdateCategoryCommand.create({
    id: formData.id,
    name: formData.name,
    type: formData.type,
    color: formData.color,
    budgetAmount: formData.budgetAmount,
    favorite: formData.favorite,
  });
  if (commandResult.isFailure) return commandResult;

  return await UpdateCategoryUseCase(repositories.categoryRepository).execute(commandResult.value);
};

export const { FormDataProvider: CategoryFormProvider, useFormDataContext: useCategoryForm } = createFormDataContext<
  CategoryFormData,
  CategoryFormError
>('Category', saveCategory);
