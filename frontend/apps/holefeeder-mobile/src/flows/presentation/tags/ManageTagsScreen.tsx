import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { DeleteTagUseCase } from '@/flows/core/tags/delete-tag/delete-tag-use-case';
import { RenameTagUseCase } from '@/flows/core/tags/rename-tag/rename-tag-use-case';
import { useTagInfos } from '@/flows/presentation/tags/core/use-tag-infos';
import { RenameTagModal } from '@/flows/presentation/tags/RenameTagModal';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppButton } from '@/shared/presentation/components/native/AppButton';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
import { AppListItem } from '@/shared/presentation/components/native/AppListItem';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppSwipeActions } from '@/shared/presentation/components/native/AppSwipeActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
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

export const ManageTagsScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const result = useTagInfos();
  const { tagRepository } = useRepositories();
  const { showDeleteAlert } = showAlert(t);
  const [editingTag, setEditingTag] = useState<string | undefined>(undefined);

  const onSave = async (newTag: string) => {
    if (editingTag === undefined) return;
    const renameResult = await RenameTagUseCase(tagRepository).execute(editingTag, newTag);
    if (renameResult.isSuccess) {
      setEditingTag(undefined);
    }
  };

  const onDelete = (tag: string) =>
    showDeleteAlert(tag, {
      onConfirm: () => {
        DeleteTagUseCase(tagRepository).execute(tag);
      },
      onCancel: () => {},
    });

  if (!result.isSuccess) {
    return (
      <AppView style={styles.container}>
        <AppLoadingIndicator />
      </AppView>
    );
  }

  if (result.value.length === 0) {
    return (
      <AppView style={styles.container}>
        <AppText variant="default">{t(tk.manageTags.empty)}</AppText>
      </AppView>
    );
  }

  return (
    <AppScreen>
      <AppNative style={{ flex: 1 }}>
        <AppList modifiers={[listStyle('inset')]}>
          <AppListForEach>
            {result.value.map((tagInfo) => (
              <AppListItem key={tagInfo.tag} onPress={() => setEditingTag(tagInfo.tag)}>
                <AppListItem.Leading>
                  <AppIcon name={AppIconMap.tag.ios} size={20} />
                </AppListItem.Leading>
                <AppSwipeActions>
                  <AppText variant="defaultSemiBold">{tagInfo.tag}</AppText>
                  <AppSwipeActions.Actions edge="trailing" allowsFullSwipe={false}>
                    <AppButton
                      variant="destructive"
                      label={t(tk.swipeableActions.delete)}
                      icon={AppIconMap.delete}
                      onPress={() => onDelete(tagInfo.tag)}
                    />
                  </AppSwipeActions.Actions>
                </AppSwipeActions>
                <AppListItem.Trailing>
                  <AppColumn alignment="end">
                    <AppText variant="footnote">{t(tk.manageTags.transactions, { count: tagInfo.transactionCount })}</AppText>
                    <AppText variant="footnote">{t(tk.manageTags.cashflows, { count: tagInfo.activeCashflowCount })}</AppText>
                  </AppColumn>
                </AppListItem.Trailing>
              </AppListItem>
            ))}
          </AppListForEach>
        </AppList>
      </AppNative>
      {editingTag !== undefined && (
        <RenameTagModal key={editingTag} tag={editingTag} isPresented onCancel={() => setEditingTag(undefined)} onSave={onSave} />
      )}
    </AppScreen>
  );
};
