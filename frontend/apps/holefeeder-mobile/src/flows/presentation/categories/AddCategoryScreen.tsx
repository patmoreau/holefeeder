import { CategoryForm } from '@/flows/presentation/categories/CategoryForm';
import { DEFAULT_CATEGORY_COLOR } from '@/flows/presentation/categories/core/category-colors';
import { CategoryFormData } from '@/flows/presentation/categories/core/category-form-data';
import { CategoryFormProvider, validateCategoryForm } from '@/flows/presentation/categories/core/use-category-form';
import { CategoryTypes } from '@/shared/core/category-type';
import { AppScreen } from '@/shared/presentation/AppScreen';

export const AddCategoryScreen = () => {
  const initialData: CategoryFormData = {
    id: null,
    name: '',
    type: CategoryTypes.expense,
    color: DEFAULT_CATEGORY_COLOR,
    budgetAmount: 0,
    favorite: false,
  };

  return (
    <AppScreen>
      <CategoryFormProvider initialValue={initialData} validate={validateCategoryForm} validateOnChange>
        <CategoryForm />
      </CategoryFormProvider>
    </AppScreen>
  );
};
