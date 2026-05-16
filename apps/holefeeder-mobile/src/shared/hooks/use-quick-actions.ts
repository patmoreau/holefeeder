import * as QuickActions from 'expo-quick-actions';
import { useQuickActionCallback } from 'expo-quick-actions/hooks';
import type { RouterAction } from 'expo-quick-actions/router';
import { router } from 'expo-router';
import { useCallback, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { Platform } from 'react-native';
import { tk } from '@/i18n/translations';
import { Logger } from '@/shared/core/logger/logger';

const log = Logger.create('use-quick-actions');

const routerInitializationDelay = 100;

type AvailableQuickActions = '/(app)/Purchase' | '/help';

const isValidQuickActionHref = (href: unknown): href is AvailableQuickActions => {
  return href === '/(app)/Purchase' || href === '/help';
};

export function useQuickActions() {
  log.debug('Initializing Quick Actions');
  const { t } = useTranslation();

  const handleQuickAction = useCallback((action: QuickActions.Action) => {
    log.debug('Callback invoked:', JSON.stringify(action, null, 2));

    if (action?.params?.href) {
      const href = action.params.href;

      if (isValidQuickActionHref(href)) {
        log.debug('Navigating to:', href);

        setTimeout(() => {
          router.push(href);
        }, routerInitializationDelay);
      } else {
        log.warn('Invalid href:', href);
      }
    } else {
      log.warn('No href found in action params');
    }
  }, []);

  useQuickActionCallback(handleQuickAction);

  useEffect(() => {
    log.debug('Setting up event listener');
    const subscription = QuickActions.addListener((event) => {
      log.debug('Event fired:', JSON.stringify(event, null, 2));
    });

    return () => {
      log.debug('Cleaning up event listener');
      subscription.remove();
    };
  }, []);

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

    setupQuickActions()
      .then(() => log.debug('Setup complete'))
      .catch((error) => log.error('Setup failed:', error));
  }, [t]);

  log.debug('Quick Actions initialized');
}
