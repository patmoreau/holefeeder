import { aTagList } from '@/flows/core/flows/__tests__/tag-list-for-test';
import { TagList } from './tag-list';

describe('TagList', () => {
  it('should remove empty and duplicate tags', () => {
    const tags = ['tag1', 'tag2', ' tag1 ', 'tag3', 'tag2 ', ''];
    const result = TagList.create(tags);
    expect(result).toBeSuccessWithValue(['tag1', 'tag2', 'tag3']);
  });

  it('should return an empty array if the input is empty', () => {
    const tags: string[] = [];
    const result = TagList.create(tags);
    expect(result).toBeSuccessWithValue([]);
  });

  describe('fromConcatenatedString', () => {
    it('should split and trim tags from a comma-separated string', () => {
      const tags = 'tag1, tag2, tag3';
      const result = TagList.fromConcatenatedString(tags);
      expect(result).toEqual(['tag1', 'tag2', 'tag3']);
    });
  });

  describe('toConcatenatedString', () => {
    it('should join tags with a comma-separated string', () => {
      const tags = aTagList();
      const result = TagList.toConcatenatedString(tags);
      expect(result).toEqual(tags.join(','));
    });
  });
});
