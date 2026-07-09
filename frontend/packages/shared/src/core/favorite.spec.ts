import { Favorite, FavoriteErrors } from './favorite';

describe('Favorite', () => {
  it('returns success for boolean true', () => {
    expect(Favorite.create(true)).toBeSuccessWithValue(true);
  });

  it('returns success for boolean false', () => {
    expect(Favorite.create(false)).toBeSuccessWithValue(false);
  });

  it('returns failure for a string', () => {
    expect(Favorite.create('true')).toBeFailureWithErrors([FavoriteErrors.invalid]);
  });

  it('returns failure for a number', () => {
    expect(Favorite.create(1)).toBeFailureWithErrors([FavoriteErrors.invalid]);
  });
});
