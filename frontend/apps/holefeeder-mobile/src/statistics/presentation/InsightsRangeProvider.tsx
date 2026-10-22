import { Logger } from '@holefeeder/shared/core';
import React, { createContext, useEffect, useState } from 'react';
import { AppStorage } from '@/shared/persistence/app-storage';
import { InsightsRangeState } from '@/statistics/presentation/core/insights-range-state';

const logger = Logger.create('InsightsRangeProvider');

const APP_SETTINGS_INSIGHTS_RANGE_KEY = 'app_settings_insights_range';

const RANGE_OPTIONS = [3, 6, 12];
const DEFAULT_RANGE = 6;

const restore = (stored: string | undefined | null): number => {
  const value = Number(stored);
  return RANGE_OPTIONS.includes(value) ? value : DEFAULT_RANGE;
};

export const InsightsRangeContext = createContext<InsightsRangeState | undefined>(undefined);

export const InsightsRangeProvider = ({ children, storage }: { children: React.ReactNode; storage: AppStorage }) => {
  const [periodCount, setPeriodCount] = useState<number>(() => restore(storage.getString(APP_SETTINGS_INSIGHTS_RANGE_KEY)));

  useEffect(() => {
    try {
      storage.setString(APP_SETTINGS_INSIGHTS_RANGE_KEY, String(periodCount));
    } catch (error) {
      logger.error('Error persisting insights range preference:', error);
    }
  }, [periodCount, storage]);

  return (
    <InsightsRangeContext.Provider value={{ periodCount, options: RANGE_OPTIONS, setPeriodCount }}>{children}</InsightsRangeContext.Provider>
  );
};
