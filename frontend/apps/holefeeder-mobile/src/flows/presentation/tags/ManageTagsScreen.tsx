import { listStyle } from '@expo/ui/swift-ui/modifiers';
import { useTranslation } from 'react-i18next';
import { useTagInfos } from '@/flows/presentation/tags/core/use-tag-infos';
import { tk } from '@/i18n/translations';
import { AppScreen } from '@/shared/presentation/AppScreen';
import { AppView } from '@/shared/presentation/AppView';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppListForEach } from '@/shared/presentation/components/native/AppListForEach';
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

export const ManageTagsScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const result = useTagInfos();

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
              <AppListItem key={tagInfo.tag}>
                <AppListItem.Leading>
                  <AppIcon name={AppIconMap.tag.ios} size={20} />
                </AppListItem.Leading>
                <AppText variant="defaultSemiBold">{tagInfo.tag}</AppText>
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
    </AppScreen>
  );
};
