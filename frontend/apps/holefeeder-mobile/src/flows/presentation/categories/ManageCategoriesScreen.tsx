import { router } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { Category } from '@/flows/core/categories/category';
import { DeactivateCategoryUseCase } from '@/flows/core/categories/deactivate/deactivate-category-use-case';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { showAlert } from '@/shared/presentation/core/show-alert';
import { useRepositories } from '@/shared/repositories/core/use-repositories';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  container: {
    ...theme.styles.containers.center,
  },
});

export const ManageCategoriesScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const result = useCategories();
  const { categoryRepository } = useRepositories();
  const { showDeleteAlert } = showAlert(t);

  const onAdd = () => router.push('/(app)/AddCategory');
  const onEdit = (category: Category) => router.push({ pathname: '/(app)/EditCategory', params: { id: category.id as string } });

  const onDelete = (category: Category) =>
    showDeleteAlert(category.name, {
      onConfirm: () => {
        DeactivateCategoryUseCase(categoryRepository).execute(category.id);
      },
      onCancel: () => {},
    });

  const toolbar = (
    <AppToolbar placement="right">
      <AppToolbarButton icon={AppIcon.select(AppIconMap.add)} accessibilityLabel={t(tk.categoryEdit.addTitle)} onPress={onAdd} />
    </AppToolbar>
  );

  if (!result.isSuccess) {
    return (
      <AppView style={styles.container}>
        {toolbar}
        <AppLoadingIndicator />
      </AppView>
    );
  }

  return (
    <AppScreen>
      {toolbar}
      <AppNative style={{ flex: 1 }}>
        <AppList inset>
          <AppListForEach>
            {result.value.map((category) => (
              <AppListItem key={category.id} onPress={category.system ? undefined : () => onEdit(category)}>
                <AppListItem.Leading>
                  <AppIcon name={AppIconMap.category.ios} size={20} color={category.color} />
                </AppListItem.Leading>
                <AppSwipeActions>
                  <AppText variant="defaultSemiBold">{category.name}</AppText>
                  {!category.system && (
                    <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={false}>
                      <AppButton
                        variant="destructive"
                        label={t(tk.swipeableActions.delete)}
                        icon={AppIconMap.delete}
                        onPress={() => onDelete(category)}
                      />
                    </AppSwipeActions.Actions>
                  )}
                </AppSwipeActions>
                <AppListItem.Trailing>
                  {category.system ? (
                    <AppText variant="footnote">{t(tk.manageCategories.system)}</AppText>
                  ) : (
                    <AppIcon name={AppIconMap.expand.ios} size={16} />
                  )}
                </AppListItem.Trailing>
              </AppListItem>
            ))}
          </AppListForEach>
        </AppList>
      </AppNative>
    </AppScreen>
  );
};
