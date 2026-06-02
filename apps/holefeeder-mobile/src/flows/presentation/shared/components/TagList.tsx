import { Id } from '@holefeeder/shared/core';
import { useTranslation } from 'react-i18next';
import { Tag } from '@/flows/core/flows/tag';
import { FilterField } from '@/flows/presentation/shared/components/FilterField';
import { useTagList } from '@/flows/presentation/shared/core/use-tag-list';
import { tk } from '@/i18n/translations';
import { AppChip } from '@/shared/presentation/components/native/AppChip';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppField } from '@/shared/presentation/components/native/AppField';
import { AppRow } from '@/shared/presentation/components/native/AppRow';
import { AppScrollView } from '@/shared/presentation/components/native/AppScrollView';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';

export type TagListProps = {
  tags: Tag[];
  selected: Tag[];
  onChange: (next: Tag[]) => void;
  categoryId: Id;
};

export function TagList({ tags, selected, onChange, categoryId }: TagListProps) {
  const { t } = useTranslation();
  const { filter, setFilter, onSubmit, toggleTag, filtered } = useTagList({ tags, selected, onChange, categoryId });

  const buildList = () =>
    filtered.map((tag) => (
      <AppChip key={tag.tag} label={`#${tag.tag}`} selected={selected.some((t) => t.tag === tag.tag)} onPress={() => toggleTag(tag)} />
    ));

  return (
    <AppField label={t(tk.purchase.basicSection.tags)} icon={AppIconMap.tag} variant="large">
      <AppColumn spacing={16}>
        <AppScrollView direction="horizontal" showsIndicators={false}>
          <AppRow spacing={8}>{filtered.length > 0 ? buildList() : <AppText variant="footnote">{t(tk.tagList.noTags)}</AppText>}</AppRow>
        </AppScrollView>
        <FilterField filter={filter} setFilter={setFilter} onSubmit={onSubmit} />
      </AppColumn>
    </AppField>
  );
}
