import { Id } from '@holefeeder/shared/core';
import { useLocalSearchParams } from 'expo-router';
import { useEffect } from 'react';
import { CategoryForm } from '@/flows/presentation/categories/CategoryForm';
import { CategoryFormData } from '@/flows/presentation/categories/core/category-form-data';
import { useCategory } from '@/flows/presentation/categories/core/use-category';
import { CategoryFormProvider, validateCategoryForm } from '@/flows/presentation/categories/core/use-category-form';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { goBack } from '@/shared/presentation/core/navigation';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const EditCategoryScreen = () => {
  const { id } = useLocalSearchParams<{ id: string }>();
  const categoryId = Id.valid(id);
  const styles = useStyles(createStyles);

  const categoryResult = useCategory(categoryId);

  // System categories are read-only; if one is reached directly, go back.
  const isSystem = categoryResult.isSuccess && categoryResult.value.system;
  useEffect(() => {
    if (isSystem) {
      goBack();
    }
  }, [isSystem]);

  if (!categoryResult.isSuccess || isSystem) {
    return (
      <AppView style={styles.container}>
        <AppLoadingIndicator />
      </AppView>
    );
  }

  const category = categoryResult.value;

  const initialData: CategoryFormData = {
    id: category.id,
    name: category.name,
    type: category.type,
    color: category.color,
    budgetAmount: category.budgetAmount,
    favorite: category.favorite,
  };

  return (
    <AppScreen>
      <CategoryFormProvider initialValue={initialData} validate={validateCategoryForm} validateOnChange>
        <CategoryForm />
      </CategoryFormProvider>
    </AppScreen>
  );
};
