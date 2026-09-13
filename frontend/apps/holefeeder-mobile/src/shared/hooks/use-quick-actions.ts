import { Logger } from '@holefeeder/shared/core';
import { router, useSegments } from 'expo-router';
import { useCallback, useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import QuickActions, { type QuickAction } from '@/modules/quick-actions';
import { useAuth } from '@/shared/auth/core/use-auth';

const log = Logger.create('use-quick-actions');

type AvailableQuickActions = '/(app)/Purchase' | '/help';

const isValidQuickActionHref = (href: unknown): href is AvailableQuickActions => {
  return href === '/(app)/Purchase' || href === '/help';
};

// Routes behind the auth guard live in the (app) group.
const PROTECTED_GROUP = '(app)';

export function useQuickActions() {
  const { t } = useTranslation();
  const { user, isLoading } = useAuth();
  const isReady = !!user && !isLoading;
  const segments = useSegments();
  // usePathname is not usable here: navigating before the protected stack mounts updates the
  // router state — so the pathname reports the target — while the screen never appears. The
  // segments report what is actually rendered, so this waits for the guarded group to be mounted.
  const isProtectedStackMounted = segments[0] === PROTECTED_GROUP;

  const handledActionRef = useRef<QuickAction | null>(null);
  const pendingHrefRef = useRef<AvailableQuickActions | null>(null);
  const [pendingSeq, setPendingSeq] = useState(0);

  const handleQuickAction = useCallback((action: QuickAction) => {
    if (handledActionRef.current === action) {
      log.debug('Ignoring re-dispatched quick action');
      return;
    }
    handledActionRef.current = action;

    const href = action?.params?.href;
    if (!isValidQuickActionHref(href)) {
      log.warn('Invalid or missing href in quick action:', JSON.stringify(action?.params));
      return;
    }

    log.debug('Queuing navigation for quick action:', href);
    pendingHrefRef.current = href;
    setPendingSeq((seq) => seq + 1);
  }, []);

  useEffect(() => {
    let isSubscribed = true;

    QuickActions.getInitialAction()
      .then((action) => {
        if (isSubscribed && action) {
          handleQuickAction(action);
        }
      })
      .catch((error) => log.error('Failed to read the launch action:', error));

    const subscription = QuickActions.addListener('onQuickAction', handleQuickAction);

    return () => {
      isSubscribed = false;
      subscription.remove();
    };
  }, [handleQuickAction]);

  // Hold the action until the guarded stack is mounted. Auth resolving is not enough: that is the
  // same commit the stack mounts in, and navigating then updates the route state without the
  // screen ever rendering. `navigate` (not `push`) is idempotent for the target route.
  useEffect(() => {
    if (!isReady || !isProtectedStackMounted || pendingHrefRef.current === null) {
      return;
    }
    const href = pendingHrefRef.current;
    pendingHrefRef.current = null;
    log.debug('Navigating to quick action target:', href);
    router.navigate(href, { withAnchor: true });
  }, [isReady, isProtectedStackMounted, pendingSeq]);

  useEffect(() => {
    const setupQuickActions = async () => {
      try {
        await QuickActions.setItems([
          {
            id: '0',
            title: t(tk.quickActions.purchaseTitle),
            icon: 'cart.fill.badge.plus',
            params: { href: '/(app)/Purchase' },
          },
          {
            id: '1',
            title: t(tk.quickActions.helpTitle),
            subtitle: t(tk.quickActions.helpSubtitle),
            icon: 'person.crop.circle.badge.questionmark',
            params: { href: '/help' },
          },
        ]);
        log.debug('Items set successfully');
      } catch (error) {
        log.error('Setup error:', error);
      }
    };

    setupQuickActions().catch((error) => log.error('Setup failed:', error));
  }, [t]);
}
