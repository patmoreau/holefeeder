import { System, SystemErrors } from './system';

describe('System', () => {
  it('returns success for boolean true', () => {
    expect(System.create(true)).toBeSuccessWithValue(true);
  });

  it('returns success for boolean false', () => {
    expect(System.create(false)).toBeSuccessWithValue(false);
  });

  it('returns failure for a string', () => {
    expect(System.create('true')).toBeFailureWithErrors([SystemErrors.invalid]);
  });

  it('returns failure for a number', () => {
    expect(System.create(1)).toBeFailureWithErrors([SystemErrors.invalid]);
  });
});
