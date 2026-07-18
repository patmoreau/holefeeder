import { renderHook, waitFor } from '@testing-library/react-native';
import { initialProfile, useProfile } from '@/settings/presentation/core/use-profile';

const mockUseAuth = jest.fn();
jest.mock('@/shared/auth/core/use-auth', () => ({
  useAuth: () => mockUseAuth(),
}));

describe('useProfile', () => {
  beforeEach(() => {
    mockUseAuth.mockReset();
  });

  it('returns the initial profile when there is no user', async () => {
    mockUseAuth.mockReturnValue({ user: null });

    const { result } = await renderHook(() => useProfile());

    await waitFor(() => expect(result.current).toEqual(initialProfile));
  });

  it('maps a fully populated user to a profile', async () => {
    mockUseAuth.mockReturnValue({
      user: { name: 'Ada Lovelace', sub: 'auth0|1', email: 'ada@example.com', picture: 'https://img/ada.png' },
    });

    const { result } = await renderHook(() => useProfile());

    await waitFor(() =>
      expect(result.current).toEqual({
        name: 'Ada Lovelace',
        username: 'auth0|1',
        email: 'ada@example.com',
        avatar: 'https://img/ada.png',
      })
    );
  });

  it('falls back to given and family name when name is absent', async () => {
    mockUseAuth.mockReturnValue({ user: { sub: 'auth0|2', givenName: 'Grace', familyName: 'Hopper' } });

    const { result } = await renderHook(() => useProfile());

    await waitFor(() => expect(result.current.name).toBe('Grace Hopper'));
  });

  it('falls back to the default avatar and empty strings when fields are absent', async () => {
    mockUseAuth.mockReturnValue({ user: { sub: 'auth0|3', givenName: 'Alan', familyName: 'Turing' } });

    const { result } = await renderHook(() => useProfile());

    await waitFor(() => expect(result.current.username).toBe('auth0|3'));
    expect(result.current.email).toBe('');
    expect(result.current.avatar).toBe('person.fill');
  });

  it('yields "undefined undefined" for name when no name fields exist (latent bug)', async () => {
    mockUseAuth.mockReturnValue({ user: { sub: 'auth0|4' } });

    const { result } = await renderHook(() => useProfile());

    // The `|| ''` fallback is dead code: the template literal is always truthy.
    await waitFor(() => expect(result.current.name).toBe('undefined undefined'));
  });
});
