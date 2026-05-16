import { Id, IdErrors } from '@/shared/core/id';

// A small set of canonical UUIDs for testing
const validUuids = [
  '00000000-0000-1000-8000-000000000000', // v1, variant 8
  '123e4567-e89b-12d3-a456-426614174000', // v1, variant a
  '550e8400-e29b-41d4-a716-446655440000', // v4, variant a
  'f47ac10b-58cc-4372-a567-0e02b2c3d479', // v4, variant a
  '3F2504E0-4F89-11D3-9A0C-0305E82C3301', // uppercase, v1, variant 9
  '6ba7b810-9dad-11d1-80b4-00c04fd430c8', // v1, variant 8
  '6ba7b811-9dad-11d1-80b4-00c04fd430c8', // v1, variant 8
  '6ba7b812-9dad-11d1-90b4-00c04fd430c8', // v1, variant 9
  '6ba7b814-9dad-11d1-a0b4-00c04fd430c8', // v1, variant a
  '6ba7b815-9dad-11d1-b0b4-00c04fd430c8', // v1, variant b
  'f2a8b0c6-7d9e-4f3a-eb6c-8d0e2f4a7b9c', // user provided UUID (variant starts with 'e')
  '550e8400-e29b-61d4-a716-446655440000', // version 6 (now accepted as we only check structure)
  '550e8400-e29b-41d4-c716-446655440000', // variant starts with 'c' (now accepted)
  '123e4567-e89b-12d3-4456-426614174000', // variant starts with '4' (now accepted)
];

const invalidUuids = [
  '', // empty
  'not-a-uuid', // random text
  '550e8400e29b41d4a716446655440000', // missing dashes
  '550e8400-e29b-41d4-g716-446655440000', // invalid hex char 'g'
  '550e8400-e29b-41d4-a716-44665544000', // too short
  '550e8400-e29b-41d4-a716-4466554400000', // too long
  '123e4567e89b12d3a456426614174000', // no dashes
];

describe('Id', () => {
  describe('create', () => {
    it.each(validUuids)('accepts a uuid (%s)', (uuid) => {
      const result = Id.create(uuid);
      expect(result).toBeSuccessWithValue(uuid);
    });

    it.each(invalidUuids)('fails for invalid UUID (%s)', (uuid) => {
      const result = Id.create(uuid as string);
      expect(result).toBeFailureWithErrors([IdErrors.invalid]);
    });

    it('is case-insensitive for hex digits', () => {
      const lower = 'a987fbc9-4bed-3078-cf07-9141ba07c9f3'; // v3 variant c (invalid variant)
      const upper = lower.toUpperCase();

      // Both should pass now that we allow any hex digit in variant position
      expect(Id.create(lower)).toBeSuccessWithValue(lower as Id);
      expect(Id.create(upper)).toBeSuccessWithValue(upper as Id);

      // A valid lowercase and uppercase should both pass
      const lowerValid = 'a987fbc9-4bed-3078-8f07-9141ba07c9f3'.toLowerCase(); // v3, variant 8
      const upperValid = lowerValid.toUpperCase();
      expect(Id.create(lowerValid)).toBeSuccessWithValue(lowerValid);
      expect(Id.create(upperValid)).toBeSuccessWithValue(upperValid);
    });
  });

  describe('newId', () => {
    it('generates a valid UUID', () => {
      const id = Id.newId();
      const result = Id.create(id);
      expect(result).toBeSuccessWithValue(id);
    });

    it('generates unique IDs', () => {
      const id1 = Id.newId();
      const id2 = Id.newId();
      const id3 = Id.newId();

      expect(id1).not.toBe(id2);
      expect(id1).not.toBe(id3);
      expect(id2).not.toBe(id3);
    });

    it('generates UUIDs in version 4 format', () => {
      const id = Id.newId();

      // UUID v4 format: xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
      // where y is one of [8, 9, a, b]
      const parts = id.split('-');
      expect(parts).toHaveLength(5);

      // Check version (13th character should be '4')
      expect(id.charAt(14)).toBe('4');

      // Check variant (17th character should be 8, 9, a, or b)
      const variantChar = id.charAt(19).toLowerCase();
      expect(['8', '9', 'a', 'b']).toContain(variantChar);
    });

    it('generates UUIDs with correct segment lengths', () => {
      const id = Id.newId();
      const parts = id.split('-');

      expect(parts[0]).toHaveLength(8);
      expect(parts[1]).toHaveLength(4);
      expect(parts[2]).toHaveLength(4);
      expect(parts[3]).toHaveLength(4);
      expect(parts[4]).toHaveLength(12);
    });

    it('generates multiple valid UUIDs in succession', () => {
      const ids = Array.from({ length: 100 }, () => Id.newId());

      ids.forEach((id) => {
        const result = Id.create(id);
        expect(result).toBeSuccessWithValue(id);
      });
    });

    it('generates UUIDs with only valid hex characters', () => {
      const id = Id.newId();
      const withoutDashes = id.replace(/-/g, '');

      // All characters should be valid hex (0-9, a-f)
      expect(withoutDashes).toMatch(/^[0-9a-f]+$/i);
    });
  });
});
