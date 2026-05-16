import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { ScrollView, View } from 'react-native';
import { Tag } from '@/flows/core/flows/tag';
import { FilterField } from '@/flows/presentation/shared/components/FilterField';
import { useTagList } from '@/flows/presentation/shared/core/use-tag-list';
import { tk } from '@/i18n/translations';
import { Id } from '@/shared/core/id';
import { AppField } from '@/shared/presentation/AppField';
import { AppChip } from '@/shared/presentation/components/AppChip';
import { AppText } from '@/shared/presentation/components/AppText';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type TagListProps = {
  tags: Tag[];
  selected: Tag[];
  onChange: (next: Tag[]) => void;
  categoryId: Id;
};

const createStyles = (theme: Theme) => ({
  container: {
    width: '100%' as const,
    flex: 1,
  },
  scrollView: {
    flex: 1,
    marginBottom: spacing.sm,
  },
  row: {
    flexDirection: 'row' as const,
    alignItems: 'center' as const,
    gap: spacing.sm,
  },
  noTags: {
    color: theme.colors.primary + '60', // More transparent
    fontStyle: 'italic' as const,
  },
});

export function TagList({ tags, selected, onChange, categoryId }: TagListProps) {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const { filter, setFilter, onSubmit, toggleTag, filtered } = useTagList({ tags, selected, onChange, categoryId });
  const [scrollViewHeight, setScrollViewHeight] = useState(36);

  const buildList = () =>
    filtered.map((tag) => (
      <AppChip key={tag.tag} label={`#${tag.tag}`} selected={selected.some((t) => t.tag === tag.tag)} onPress={() => toggleTag(tag)} />
    ));

  return (
    <AppField label={t(tk.purchase.basicSection.tags)} icon={AppIcons.tag} variant="large">
      <View style={styles.container}>
        <ScrollView
          horizontal
          showsHorizontalScrollIndicator={false}
          contentContainerStyle={styles.row}
          style={[styles.scrollView, { height: scrollViewHeight }]}
          onLayout={(event) => setScrollViewHeight(event.nativeEvent.layout.height)}
        >
          {filtered.length > 0 ? (
            buildList()
          ) : (
            <AppText variant="footnote" style={styles.noTags}>
              {t(tk.tagList.noTags)}
            </AppText>
          )}
        </ScrollView>
        <FilterField filter={filter} setFilter={setFilter} onSubmit={onSubmit} />
      </View>
    </AppField>
  );
}
