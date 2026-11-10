import { Logger } from '@holefeeder/shared/core';
import React, { createContext, useEffect, useState } from 'react';
import { AppState } from 'react-native';
import { BillsDueBadgeState } from '@/shared/bills-due-badge/core/bills-due-badge-state';
import {
  NotificationPermission,
  NotificationPermissionStatus,
  NotificationPermissionStatuses,
} from '@/shared/bills-due-badge/core/notification-permission';
import { AppStorage } from '@/shared/persistence/app-storage';

const logger = Logger.create('BillsDueBadgeProvider');

const APP_SETTINGS_BILLS_DUE_BADGE_KEY = 'app_settings_bills_due_badge';

export const BillsDueBadgeContext = createContext<BillsDueBadgeState | undefined>(undefined);

type BillsDueBadgeProviderProps = {
  children: React.ReactNode;
  storage: AppStorage;
  permission: NotificationPermission;
};

export const BillsDueBadgeProvider = ({ children, storage, permission }: BillsDueBadgeProviderProps) => {
  const [saved, setSaved] = useState<boolean>(() => storage.getString(APP_SETTINGS_BILLS_DUE_BADGE_KEY) === 'true');
  const [status, setStatus] = useState<NotificationPermissionStatus>(NotificationPermissionStatuses.undetermined);

  useEffect(() => {
    const refresh = () =>
      permission.status().then((result) => {
        if (result.isSuccess) setStatus(result.value);
        else logger.error('Error checking notification permission:', result.errors);
      });
    refresh();
    const subscription = AppState.addEventListener('change', (appState) => {
      if (appState === 'active') refresh();
    });
    return () => subscription.remove();
  }, [permission]);

  const save = (value: boolean) => {
    storage.setString(APP_SETTINGS_BILLS_DUE_BADGE_KEY, String(value));
    setSaved(value);
  };

  const allow = async (): Promise<NotificationPermissionStatus | undefined> => {
    const current = await permission.status();
    if (current.isSuccess && current.value === NotificationPermissionStatuses.granted) return current.value;
    const requested = await permission.request();
    return requested.isSuccess ? requested.value : undefined;
  };

  const setEnabled = async (value: boolean) => {
    if (!value) {
      save(false);
      return;
    }
    const allowed = await allow();
    if (allowed === undefined) return;
    setStatus(allowed);
    if (allowed === NotificationPermissionStatuses.granted) save(true);
  };

  const state: BillsDueBadgeState = {
    enabled: saved && status === NotificationPermissionStatuses.granted,
    denied: status === NotificationPermissionStatuses.denied,
    setEnabled: setEnabled,
    openSettings: async () => {
      await permission.openSettings();
    },
  };

  return <BillsDueBadgeContext.Provider value={state}>{children}</BillsDueBadgeContext.Provider>;
};
