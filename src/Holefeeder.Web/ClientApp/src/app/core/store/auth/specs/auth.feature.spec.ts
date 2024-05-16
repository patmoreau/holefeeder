import { AuthActions } from '../auth.actions';
import { AuthFeature, AuthState, initialState } from '../auth.feature';
import { User } from '@app/shared/models';

describe('Auth Feature', () => {
  it('[Auth API] Login Complete', () => {
    // arrange
    const user = new User('Doe', 'John', 'John Doe', '1');
    const action = AuthActions.loginComplete({
      isAuthenticated: true,
      profile: user,
    });

    const expectedState: AuthState = {
      isAuthenticated: true,
      profile: user,
    };

    // act
    const result = AuthFeature.reducer(initialState, action);

    // assert
    expect(result).toEqual(expectedState);
  });

  it('[Auth API] Logout', () => {
    // arrange
    const action = AuthActions.logout();

    const expectedState: AuthState = {
      isAuthenticated: false,
      profile: null,
    };

    // act
    const result = AuthFeature.reducer(initialState, action);

    // assert
    expect(result).toEqual(expectedState);
  });
});
