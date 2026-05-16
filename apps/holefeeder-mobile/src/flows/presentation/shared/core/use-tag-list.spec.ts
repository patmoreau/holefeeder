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

  beforeEach(() => {
    mockOnChange.mockClear();
    hookResult = renderHook(
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
    it('toggle to selected on pressing an unselected tag', () => {
      act(() => {
        hookResult.result.current.toggleTag(middleTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([selectedTag, middleTag]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [selectedTag, middleTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag, firstTag, lastTag]);
    });

    it('toggle to unselected on pressing a selected tag', () => {
      act(() => {
        hookResult.result.current.toggleTag(selectedTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([middleTag, firstTag, lastTag, selectedTag]);
    });
  });

  describe('when entering text', () => {
    it('shows tags matching the text', () => {
      act(() => {
        hookResult.result.current.setFilter('d');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag]);
    });

    it('trim spaces', () => {
      act(() => {
        hookResult.result.current.setFilter(' d ');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag]);
    });

    it('shows no tags on no match', () => {
      act(() => {
        hookResult.result.current.setFilter('z');
      });

      expect(hookResult.result.current.filtered).toStrictEqual([]);
    });

    it('on enter with exact single match, selects that tag and clears filter', () => {
      act(() => {
        hookResult.result.current.setFilter('mid');
      });

      act(() => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([middleTag, selectedTag]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [middleTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([middleTag, selectedTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on pressing a tag, selects that tag and clears filter', () => {
      act(() => {
        hookResult.result.current.setFilter('mid');
      });

      act(() => {
        hookResult.result.current.toggleTag(middleTag);
      });

      expect(mockOnChange).toHaveBeenCalledWith([selectedTag, middleTag]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [selectedTag, middleTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on enter with multiple matches, creates a new lowercase tag and selects it, then clears filter', () => {
      act(() => {
        hookResult.result.current.setFilter(dTag.tag);
      });

      act(() => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([dTag, selectedTag]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [dTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([dTag, selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('on enter, add new tag to list and selects it', () => {
      act(() => {
        hookResult.result.current.setFilter(newTag.tag.toUpperCase());
      });

      act(() => {
        hookResult.result.current.onSubmit();
      });

      expect(mockOnChange).toHaveBeenCalledWith([newTag, selectedTag]);

      hookResult.rerender({
        tags: [firstTag, middleTag, lastTag, selectedTag],
        selected: [newTag, selectedTag],
        onChange: mockOnChange,
      });

      expect(hookResult.result.current.filtered).toStrictEqual([newTag, selectedTag, middleTag, firstTag, lastTag]);
      expect(hookResult.result.current.filter).toBe('');
    });

    it('handles tag list updates with same IDs but different references (database refresh)', () => {
      // simulating database refresh where we get new objects for the same tags
      const newRefMiddleTag = { ...middleTag };
      const newRefFirstTag = { ...firstTag };
      const newRefSelectedTag = { ...selectedTag };

      hookResult.rerender({
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
