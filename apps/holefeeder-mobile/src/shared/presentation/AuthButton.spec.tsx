import { act, fireEvent, render, screen, waitFor } from '@testing-library/react-native';
import { anId } from '@/shared/__tests__/string-for-test';
import { AuthenticationContextForTest } from '@/shared/auth/__tests__/AuthenticationContextForTest';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

xdescribe('<AuthButton />', () => {
  beforeEach(() => {});

  describe('when user is logged', () => {
    const mockLogout = jest.fn();

    beforeEach(async () => {
      await waitFor(() =>
        render(
          <ThemeProviderForTest>
            <AuthenticationContextForTest
              overrides={{
                user: { sub: anId() },
                isLoading: false,
              }}
            >
              <AuthButton />
            </AuthenticationContextForTest>
          </ThemeProviderForTest>
        )
      );
    });

    it('shows logout button', () => {
      expect(screen.queryByRole('button', { name: /auth.logoutButton/i })).toBeOnTheScreen();
    });

    it('invokes logout method when clicked', () => {
      act(() => fireEvent.press(screen.queryByRole('button', { name: /auth.logoutButton/i })));

      expect(mockLogout).toHaveBeenCalled();
    });
  });

  describe('when user not user is logged', () => {
    const mockLogin = jest.fn();

    beforeEach(async () => {
      await waitFor(() =>
        render(
          <ThemeProviderForTest>
            <AuthenticationContextForTest>
              <AuthButton />
            </AuthenticationContextForTest>
          </ThemeProviderForTest>
        )
      );
    });

    it('shows login button', async () => {
      expect(screen.queryByRole('button', { name: /auth.loginButton/i })).toBeOnTheScreen();
    });

    it('invokes login method when clicked', () => {
      act(() => fireEvent.press(screen.queryByRole('button', { name: /auth.loginButton/i })));

      expect(mockLogin).toHaveBeenCalled();
    });
  });
});
