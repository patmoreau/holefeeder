import { Icon } from '@expo/ui';
import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { router, Stack } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { Category } from '@/flows/core/categories/category';
import { useCategories } from '@/flows/presentation/shared/core/use-categories';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
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

  const onAdd = () => router.push('/(app)/AddCategory');
  const onEdit = (category: Category) => router.push({ pathname: '/(app)/EditCategory', params: { id: category.id as string } });

  const toolbar = (
    <Stack.Toolbar placement="right">
      <Stack.Toolbar.Button icon={Icon.select(AppIconMap.add)} onPress={onAdd} />
    </Stack.Toolbar>
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
        <AppList modifiers={[listStyle('inset')]}>
          {result.value.map((category) => (
            <AppListItem key={category.id} onPress={category.system ? undefined : () => onEdit(category)}>
              <AppListItem.Leading>
                <AppIcon name={AppIconMap.category.ios} size={20} color={category.color} />
              </AppListItem.Leading>
              <AppText variant="defaultSemiBold">{category.name}</AppText>
              <AppListItem.Trailing>
                {category.system ? (
                  <AppText variant="footnote">{t(tk.manageCategories.system)}</AppText>
                ) : (
                  <AppIcon name={AppIconMap.expand.ios} size={16} />
                )}
              </AppListItem.Trailing>
            </AppListItem>
          ))}
        </AppList>
      </AppNative>
    </AppScreen>
  );
};
