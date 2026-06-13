import { Boolean, BooleanErrors } from './boolean';

describe('boolean', () => {
  it('returns success for boolean true', () => {
    expect(Boolean.create(true)).toBeSuccessWithValue(Boolean.True);
  });

  it('returns success for boolean false', () => {
    expect(Boolean.create(false)).toBeSuccessWithValue(Boolean.False);
  });

  it('returns failure for invalid type', () => {
    expect(Boolean.create('true')).toBeFailureWithErrors([BooleanErrors.type]);
  });

  it('Boolean.True is a branded true', () => {
    expect(Boolean.True).toBe(true);
  });

  it('Boolean.False is a branded false', () => {
    expect(Boolean.False).toBe(false);
  });
});
