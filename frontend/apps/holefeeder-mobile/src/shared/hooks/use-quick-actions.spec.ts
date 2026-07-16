import { act, renderHook } from '@testing-library/react-native';
import type { Action } from 'expo-quick-actions';
import { router } from 'expo-router';
import { useQuickActions } from '@/shared/hooks/use-quick-actions';

// Capture the callback the hook registers with expo-quick-actions so tests can
// simulate the OS dispatching a quick action (both the cold-launch "initial"
// action and a warm listener event). The handler is stable across renders, so
// the latest registered callback is the one to invoke.
const mockUseQuickActionCallback = jest.fn();
jest.mock('expo-quick-actions/hooks', () => ({
  useQuickActionCallback: (cb: (action: Action) => void) => mockUseQuickActionCallback(cb),
}));

const mockSetItems = jest.fn().mockResolvedValue(undefined);
jest.mock('expo-quick-actions', () => ({
  setItems: (...args: unknown[]) => mockSetItems(...args),
  addListener: jest.fn(() => ({ remove: jest.fn() })),
  initial: null,
}));

jest.mock('expo-router', () => ({
  router: {
    navigate: jest.fn(),
    push: jest.fn(),
  },
}));

jest.mock('react-i18next', () => ({
  useTranslation: () => ({
    t: (key: string) => key,
  }),
}));

const mockUseAuth = jest.fn();
jest.mock('@/shared/auth/core/use-auth', () => ({
  useAuth: () => mockUseAuth(),
}));

const mockNavigate = router.navigate as jest.Mock;
const mockPush = router.push as jest.Mock;

const purchaseAction = (overrides: Partial<Action> = {}): Action =>
  ({ id: '0', title: 'Purchase', params: { href: '/(app)/Purchase' }, ...overrides }) as Action;

const capturedCallback = (): ((action: Action) => void) => mockUseQuickActionCallback.mock.calls.at(-1)![0];

const dispatch = async (action: Action) => {
  await act(async () => {
    capturedCallback()(action);
  });
};

const renderReady = async () => {
  mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
  return renderHook(() => useQuickActions());
};

describe('useQuickActions', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
  });

  describe('navigation', () => {
    it('should navigate to the target route (not push) for a valid action', async () => {
      await renderReady();

      await dispatch(purchaseAction());

      expect(mockNavigate).toHaveBeenCalledTimes(1);
      expect(mockNavigate).toHaveBeenCalledWith('/(app)/Purchase', { withAnchor: true });
      expect(mockPush).not.toHaveBeenCalled();
    });

    it('should navigate to the help route for the help action', async () => {
      await renderReady();

      await dispatch(purchaseAction({ id: '1', params: { href: '/help' } }));

      expect(mockNavigate).toHaveBeenCalledWith('/help', { withAnchor: true });
    });

    it('should ignore an action with an invalid href', async () => {
      await renderReady();

      await dispatch(purchaseAction({ params: { href: '/somewhere-else' } }));

      expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should ignore an action with no params', async () => {
      await renderReady();

      await dispatch(purchaseAction({ params: undefined }));

      expect(mockNavigate).not.toHaveBeenCalled();
    });
  });

  describe('deduplication of a re-dispatched launch action', () => {
    it('should navigate only once when the same action object is dispatched repeatedly', async () => {
      await renderReady();
      const action = purchaseAction();

      await dispatch(action);
      await dispatch(action);
      await dispatch(action);

      expect(mockNavigate).toHaveBeenCalledTimes(1);
    });

    it('should navigate again for a distinct dispatch of the same route', async () => {
      await renderReady();

      await dispatch(purchaseAction());
      await dispatch(purchaseAction());

      expect(mockNavigate).toHaveBeenCalledTimes(2);
    });
  });

  describe('auth readiness gating', () => {
    it('should not navigate when a launch action arrives before the user is authenticated', async () => {
      mockUseAuth.mockReturnValue({ user: null, isLoading: true });
      await renderHook(() => useQuickActions());

      await dispatch(purchaseAction());

      expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should navigate the pending action once the user becomes authenticated', async () => {
      mockUseAuth.mockReturnValue({ user: null, isLoading: true });
      const { rerender } = await renderHook(() => useQuickActions());

      await dispatch(purchaseAction());
      expect(mockNavigate).not.toHaveBeenCalled();

      mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
      await act(async () => {
        rerender({});
      });

      expect(mockNavigate).toHaveBeenCalledTimes(1);
      expect(mockNavigate).toHaveBeenCalledWith('/(app)/Purchase', { withAnchor: true });
    });
  });

  describe('registration', () => {
    it('should register the purchase and help quick actions', async () => {
      await renderReady();

      expect(mockSetItems).toHaveBeenCalledTimes(1);
      const items = mockSetItems.mock.calls[0][0];
      expect(items).toHaveLength(2);
      expect(items.map((item: { params: { href: string } }) => item.params.href)).toEqual(['/(app)/Purchase', '/help']);
    });
  });
});
