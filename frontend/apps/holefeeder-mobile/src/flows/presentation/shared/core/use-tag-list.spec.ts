import { act, renderHook, RenderHookResult } from '@testing-library/react-native';
import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { Tag } from '@/flows/core/flows/tag';
import { anId } from '@/shared/__tests__/string-for-test';
import { useTagList } from './use-tag-list';

const categoryId = anId();
const firstTag = aTag({ tag: 'first-tag', count: 1, categoryId: categoryId });
const middleTag = aTag({ tag: 'middle-tag', count: 2, categoryId: categoryId });
const lastTag = aTag({ tag: 'last-tag', count: 10, categoryId: anId() });
const selectedTag = aTag({ tag: 'selected-tag', count: 4, categoryId: anId() });
const newTag = aTag({ tag: 'new-tag', count: 0, categoryId: undefined });
const dTag = aTag({ tag: 'd', count: 0, categoryId: undefined });

describe('useTagList', () => {
  const mockOnChange = jest.fn();
  let hookResult: RenderHookResult<ReturnType<typeof useTagList>, { tags: Tag[]; selected: Tag[]; onChange: (next: Tag[]) => void }>;

  beforeEach(async () => {
    mockOnChange.mockClear();
    hookResult = await renderHook(
      ({ tags, selected, onChange }) =>
        useTagList({
          tags,
          selected,
          onChange,
          categoryId,
        }),
      {
        initialProps: {
          tags: [firstTag, middleTag, lastTag, selectedTag],
          selected: [selectedTag],
          onChange: mockOnChange,
        },
      }
    );
  });

  it('initializes to empty filter', () => {
    expect(hookResult.result.current.filter).toBe('');
  });

  it('initializes to ordered list', () => {
    expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag, firstTag, lastTag]);
  });

  describe('when toggling a tag', () => {
    it('toggle to selected on pressing an unselected tag', async () => {
      await act(async () => {
        hookResult.result.current.toggleTag(middleTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([selectedTag, middleTag]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [selectedTag, middleTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag, firstTag, lastTag]);
    });

    it('toggle to unselected on pressing a selected tag', async () => {
      await act(async () => {
        hookResult.result.current.toggleTag(selectedTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([middleTag, firstTag, lastTag, selectedTag]);
    });
  });

  describe('when entering text', () => {
    it('shows tags matching the text', async () => {
      await act(async () => {
        hookResult.result.current.setFilter('d');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag]);
    });

    it('trim spaces', async () => {
      await act(async () => {
        hookResult.result.current.setFilter(' d ');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag]);
    });

    it('shows no tags on no match', async () => {
      await act(async () => {
        hookResult.result.current.setFilter('z');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([]);
    });

    it('on enter with exact single match, selects that tag and clears filter', async () => {
      await act(async () => {
        hookResult.result.current.setFilter('mid');
      });

      await act(async () => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([middleTag, selectedTag]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [middleTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([middleTag, selectedTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on pressing a tag, selects that tag and clears filter', async () => {
      await act(async () => {
        hookResult.result.current.setFilter('mid');
      });

      await act(async () => {
        hookResult.result.current.toggleTag(middleTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([selectedTag, middleTag]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [selectedTag, middleTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on enter with multiple matches, creates a new lowercase tag and selects it, then clears filter', async () => {
      await act(async () => {
        hookResult.result.current.setFilter(dTag.tag);
      });

      await act(async () => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([dTag, selectedTag]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [dTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([dTag, selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on enter, add new tag to list and selects it', async () => {
      await act(async () => {
        hookResult.result.current.setFilter(newTag.tag.toUpperCase());
      });

      await act(async () => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([newTag, selectedTag]);

      await hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [newTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([newTag, selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('handles tag list updates with same IDs but different references (database refresh)', async () => {
      // simulating database refresh where we get new objects for the same tags
      const newRefMiddleTag = { ...middleTag };
      const newRefFirstTag = { ...firstTag };
      const newRefSelectedTag = { ...selectedTag };

      await hookResult.rerender({
        tags: [newRefFirstTag, newRefMiddleTag, lastTag, newRefSelectedTag],
        selected: [selectedTag], // selected stays the same (from local state usually)
        onChange: mockOnChange,
      });

      // Should still be unique by ID
      // If deduplication fails, we might see duplicates here
      const seenIds = new Set();
      const duplicates = hookResult.result.current.filtered.filter((t) => {
        if (seenIds.has(t.tag)) return true;
        seenIds.add(t.tag);
        return false;
      });

      expect(duplicates).toHaveLength(0);
      expect(hookResult.result.current.filtered).toHaveLength(4);
    });
  });
});
