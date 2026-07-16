import { Logger } from '@holefeeder/shared/core';
import * as QuickActions from 'expo-quick-actions';
import { useQuickActionCallback } from 'expo-quick-actions/hooks';
import type { RouterAction } from 'expo-quick-actions/router';
import { router } from 'expo-router';
import { useCallback, useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Platform } from 'react-native';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';

const log = Logger.create('use-quick-actions');

type AvailableQuickActions = '/(app)/Purchase' | '/help';

const isValidQuickActionHref = (href: unknown): href is AvailableQuickActions => {
  return href === '/(app)/Purchase' || href === '/help';
};

export function useQuickActions() {
  const { t } = useTranslation();
  const { user, isLoading } = useAuth();
  const isReady = !!user && !isLoading;

  // The action that cold-launched the app (`QuickActions.initial`) is a stable object that is
  // never cleared, and the library re-dispatches it whenever its effect re-runs. Deduplicating by
  // object identity lets a genuine re-tap (a fresh event object) through while ignoring re-fires of
  // the same launch action, so we never yank the user back to Purchase after they've navigated away.
  const handledActionRef = useRef<QuickActions.Action | null>(null);
  const pendingHrefRef = useRef<AvailableQuickActions | null>(null);
  const [pendingSeq, setPendingSeq] = useState(0);

  const handleQuickAction = useCallback((action: QuickActions.Action) => {
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

  useQuickActionCallback(handleQuickAction);

  // Defer navigation until the app is authenticated and the protected routes are mounted. On a cold
  // launch the action fires before auth resolves; navigating then would race an unmounted route
  // stack. `navigate` (not `push`) is idempotent for the target route, so it never stacks duplicate
  // Purchase screens the way the previous `router.push` did.
  useEffect(() => {
    if (!isReady || pendingHrefRef.current === null) {
      return;
    }
    const href = pendingHrefRef.current;
    pendingHrefRef.current = null;
    log.debug('Navigating to quick action target:', href);
    router.navigate(href, { withAnchor: true });
  }, [isReady, pendingSeq]);

  useEffect(() => {
    const setupQuickActions = async () => {
      try {
        await QuickActions.setItems<RouterAction<AvailableQuickActions>>([
          {
            title: t(tk.quickActions.purchaseTitle),
            icon: Platform.OS === 'ios' ? 'symbol:cart.fill.badge.plus' : undefined,
            id: '0',
            params: { href: '/(app)/Purchase' },
          },
          {
            title: t(tk.quickActions.helpTitle),
            subtitle: t(tk.quickActions.helpSubtitle),
            icon: Platform.OS === 'ios' ? 'symbol:person.crop.circle.badge.questionmark' : undefined,
            id: '1',
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
