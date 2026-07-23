import { Logger } from '@holefeeder/shared/core';
import React, { createContext, useEffect, useState } from 'react';
import { AppStorage } from '@/shared/persistence/app-storage';
import { CombinedInsightState } from '@/statistics/presentation/core/combined-insight-state';

const logger = Logger.create('CombinedInsightProvider');

const APP_SETTINGS_COMBINED_INSIGHT_KEY = 'app_settings_combined_insight';

export const CombinedInsightContext = createContext<CombinedInsightState | undefined>(undefined);

export const CombinedInsightProvider = ({ children, storage }: { children: React.ReactNode; storage: AppStorage }) => {
  const [combined, setCombined] = useState<boolean>(() => storage.getString(APP_SETTINGS_COMBINED_INSIGHT_KEY) === 'true');

  useEffect(() => {
    try {
      storage.setString(APP_SETTINGS_COMBINED_INSIGHT_KEY, String(combined));
    } catch (error) {
      logger.error('Error persisting combined insight preference:', error);
    }
  }, [combined, storage]);

  return <CombinedInsightContext.Provider value={{ combined, setCombined }}>{children}</CombinedInsightContext.Provider>;
};
