import { useCallback, useMemo, useState } from 'react';
import { Tag } from '@/flows/core/flows/tag';

export const useTagList = ({
  tags,
  selected,
  onChange,
  categoryId,
}: {
  tags: Tag[];
  selected: Tag[];
  onChange: (next: Tag[]) => void;
  categoryId?: string;
}) => {
  const [filter, setFilter] = useState('');

  const ordered = useMemo(() => {
    const selectedSet = new Set(selected.map((t) => t.tag));

    const tagMap = new Map<string, { total: number; categoryCount: number }>();
    for (const t of tags) {
      if (selectedSet.has(t.tag)) continue;

      const existing = tagMap.get(t.tag) || { total: 0, categoryCount: 0 };
      existing.total += t.count;
      if (categoryId && t.categoryId === categoryId) {
        existing.categoryCount += t.count;
      }
      tagMap.set(t.tag, existing);
    }

    const unselected = Array.from(tagMap.entries()).map(([tag, counts]) => ({
      tag,
      total: counts.total,
      categoryCount: counts.categoryCount,
    }));

    unselected.sort((a, b) => {
      if (a.categoryCount !== b.categoryCount) {
        return b.categoryCount - a.categoryCount;
      }
      if (a.total !== b.total) {
        return b.total - a.total;
      }
      return a.tag.localeCompare(b.tag);
    });

    const unselectedTags = unselected.map((u) => {
      const original = tags.find((t) => t.tag === u.tag)!;
      return Tag.valid({ ...original, count: u.total });
    });

    return [...selected, ...unselectedTags];
  }, [tags, selected, categoryId]);

  const filtered = useMemo(() => {
    const q = filter.trim().toLowerCase();
    if (!q) return ordered;
    return ordered.filter((t) => t.tag.toLowerCase().includes(q));
  }, [ordered, filter]);

  const toggleTag = useCallback(
    (t: Tag) => {
      const exists = selected.includes(t);
      const next = exists ? selected.filter((s) => s !== t) : [...selected, t];
      onChange(next);
      setFilter('');
    },
    [selected, onChange]
  );

  const onSubmit = useCallback(() => {
    const q = filter.trim().toLowerCase();
    if (!q) return;

    const matches = tags.filter((t) => t.tag.toLowerCase().includes(q));

    let nextSelected = selected;
    if (matches.length === 1) {
      const tag = matches[0];
      if (!selected.some((t) => t.tag === tag.tag)) {
        nextSelected = [tag, ...selected];
      }
    } else {
      const newTag = Tag.create({ tag: q, count: 0 });
      if (newTag.isSuccess && !selected.some((t) => t.tag === newTag.value.tag)) {
        nextSelected = [newTag.value, ...selected];
      }
    }

    if (nextSelected !== selected) {
      onChange(nextSelected);
    }

    setFilter('');
  }, [filter, tags, selected, onChange]);

  return {
    filter,
    setFilter,
    onSubmit,
    toggleTag,
    filtered,
  };
};
