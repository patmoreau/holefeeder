import { Logger } from '@holefeeder/shared/core';
import { router, usePathname } from 'expo-router';
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

// usePathname reports the resolved route, which omits group segments like (app).
const routeFor = (href: AvailableQuickActions) => href.replace(/\/\([^)]*\)/g, '');

export function useQuickActions() {
  const { t } = useTranslation();
  const { user, isLoading } = useAuth();
  const isReady = !!user && !isLoading;
  const pathname = usePathname();

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

  // Defer navigation until the app is authenticated, then keep the request pending until the
  // pathname confirms arrival. On a cold launch the protected route stack mounts after auth
  // resolves, so the first navigate can land before the target route exists and is silently
  // dropped; re-running as the pathname settles retries it. `navigate` (not `push`) is idempotent
  // for the target route, so a retry never stacks duplicate Purchase screens.
  useEffect(() => {
    if (!isReady || pendingHrefRef.current === null) {
      return;
    }
    const href = pendingHrefRef.current;
    if (pathname === routeFor(href)) {
      log.debug('Quick action target reached:', pathname);
      pendingHrefRef.current = null;
      return;
    }

    log.debug('Navigating to quick action target:', href);
    router.navigate(href, { withAnchor: true });
  }, [isReady, pendingSeq, pathname]);

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
