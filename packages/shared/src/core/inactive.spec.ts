import { Inactive, InactiveErrors } from './inactive';

describe('Inactive', () => {
  it('returns success for boolean true', () => {
    expect(Inactive.create(true)).toBeSuccessWithValue(true);
  });

  it('returns success for boolean false', () => {
    expect(Inactive.create(false)).toBeSuccessWithValue(false);
  });

  it('returns failure for a string', () => {
    expect(Inactive.create('true')).toBeFailureWithErrors([InactiveErrors.invalid]);
  });

  it('returns failure for a number', () => {
    expect(Inactive.create(1)).toBeFailureWithErrors([InactiveErrors.invalid]);
  });
});
