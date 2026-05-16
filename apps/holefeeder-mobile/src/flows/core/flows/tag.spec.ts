import { aTag } from '@/flows/core/flows/__tests__/tag-for-test';
import { Tag, TagErrors } from './tag';

describe('Tag', () => {
  const validTag = aTag();

  describe('create', () => {
    it('create a valid tag', () => {
      const result = Tag.create(validTag);

      expect(result).toBeSuccessWithValue(validTag);
    });

    it('rejects invalid tag (empty)', () => {
      const result = Tag.create({ ...validTag, tag: '' });
      expect(result).toBeFailureWithErrors([TagErrors.invalidName]);
    });

    it('rejects invalid tag (wrong type)', () => {
      const result = Tag.create({ ...validTag, tag: 123 });
      expect(result).toBeFailureWithErrors([TagErrors.invalidName]);
    });

    it('rejects invalid count', () => {
      const result = Tag.create({ ...validTag, count: 'invalid-count' });
      expect(result).toBeFailureWithErrors([TagErrors.invalidCount]);
    });
  });

  describe('valid', () => {
    it('create a valid tag', () => {
      const result = Tag.valid(validTag);

      expect(result).toEqual(validTag);
    });
  });
});
