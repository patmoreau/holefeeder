import { act, renderHook } from '@testing-library/react-native';
import { router } from 'expo-router';
import type { QuickAction } from '@/modules/quick-actions';
import { useQuickActions } from '@/shared/hooks/use-quick-actions';

const mockSetItems = jest.fn().mockResolvedValue(undefined);
const mockGetInitialAction = jest.fn().mockResolvedValue(null);
const mockAddListener = jest.fn((_event: string, _listener: (action: QuickAction) => void) => ({ remove: jest.fn() }));

jest.mock('@/modules/quick-actions', () => ({
  __esModule: true,
  default: {
    setItems: (...args: unknown[]) => mockSetItems(...args),
    getInitialAction: () => mockGetInitialAction(),
    addListener: (event: string, listener: (action: QuickAction) => void) => mockAddListener(event, listener),
  },
}));

const mockSegments = jest.fn<string[], []>(() => ['(app)', '(tabs)']);
jest.mock('expo-router', () => ({
  router: {
    navigate: jest.fn(),
    push: jest.fn(),
  },
  useSegments: () => mockSegments(),
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

const purchaseAction = (overrides: Partial<QuickAction> = {}): QuickAction => ({
  id: '0',
  title: 'Purchase',
  params: { href: '/(app)/Purchase' },
  ...overrides,
});

// The hook subscribes through the module's own event listener, so the most
// recently registered callback is the one the OS would dispatch to.
const capturedListener = (): ((action: QuickAction) => void) => mockAddListener.mock.calls.at(-1)![1];

const dispatch = async (action: QuickAction) => {
  await act(async () => {
    capturedListener()(action);
  });
};

const renderReady = async () => {
  mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
  // renderHook is async here, and awaiting it also settles the pending
  // getInitialAction promise the hook kicks off on mount.
  return await renderHook(() => useQuickActions());
};

describe('useQuickActions', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    mockSegments.mockReturnValue(['(app)', '(tabs)']);
    mockGetInitialAction.mockResolvedValue(null);
    mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
  });

  describe('cold launch', () => {
    it('should navigate to the action the app was launched with', async () => {
      mockGetInitialAction.mockResolvedValue(purchaseAction());

      await renderReady();

      expect(mockNavigate).toHaveBeenCalledWith('/(app)/Purchase', { withAnchor: true });
    });

    it('should ask the native module for the launch action exactly once', async () => {
      await renderReady();

      expect(mockGetInitialAction).toHaveBeenCalledTimes(1);
    });

    it('should not navigate when the app was not launched from a quick action', async () => {
      mockGetInitialAction.mockResolvedValue(null);

      await renderReady();

      expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should hold the launch action until the user is authenticated', async () => {
      mockGetInitialAction.mockResolvedValue(purchaseAction());
      mockUseAuth.mockReturnValue({ user: null, isLoading: true });
      const { rerender } = await renderHook(() => useQuickActions());

      expect(mockNavigate).not.toHaveBeenCalled();

      mockUseAuth.mockReturnValue({ user: { id: 'user-1' }, isLoading: false });
      await act(async () => {
        rerender({});
      });

      expect(mockNavigate).toHaveBeenCalledWith('/(app)/Purchase', { withAnchor: true });
    });
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

  describe('deduplication of a re-dispatched action', () => {
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
    it('should not navigate when an action arrives before the user is authenticated', async () => {
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

  describe('waiting for the guarded stack', () => {
    it('should not navigate while the protected group is not mounted', async () => {
      mockSegments.mockReturnValue(['index']);

      await renderReady();
      await dispatch(purchaseAction());

      expect(mockNavigate).not.toHaveBeenCalled();
    });

    it('should navigate once the protected group mounts', async () => {
      mockSegments.mockReturnValue(['index']);
      const { rerender } = await renderReady();
      await dispatch(purchaseAction());
      expect(mockNavigate).not.toHaveBeenCalled();

      mockSegments.mockReturnValue(['(app)', '(tabs)']);
      await act(async () => {
        rerender({});
      });

      expect(mockNavigate).toHaveBeenCalledTimes(1);
      expect(mockNavigate).toHaveBeenCalledWith('/(app)/Purchase', { withAnchor: true });
    });

    it('should navigate only once as the stack continues to settle', async () => {
      const { rerender } = await renderReady();
      await dispatch(purchaseAction());

      mockSegments.mockReturnValue(['(app)', 'Purchase']);
      await act(async () => {
        rerender({});
      });

      expect(mockNavigate).toHaveBeenCalledTimes(1);
    });
  });

  describe('registration', () => {
    it('should register the purchase and help quick actions', async () => {
      await renderReady();

      expect(mockSetItems).toHaveBeenCalledTimes(1);
      const items = mockSetItems.mock.calls[0][0];
      expect(items).toHaveLength(2);
      expect(items.map((item: QuickAction) => item.params?.href)).toEqual(['/(app)/Purchase', '/help']);
    });

    it('should register icons as bare SF Symbol names', async () => {
      await renderReady();

      const items = mockSetItems.mock.calls[0][0];
      expect(items.map((item: QuickAction) => item.icon)).toEqual(['cart.fill.badge.plus', 'person.crop.circle.badge.questionmark']);
    });
  });
});
