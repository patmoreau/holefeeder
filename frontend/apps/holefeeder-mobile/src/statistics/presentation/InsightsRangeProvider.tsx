import { InsightsRange, Logger } from '@holefeeder/shared/core';
import React, { createContext, useEffect, useState } from 'react';
import { AppStorage } from '@/shared/persistence/app-storage';
import { InsightsRangeState } from '@/statistics/presentation/core/insights-range-state';

const logger = Logger.create('InsightsRangeProvider');

const APP_SETTINGS_INSIGHTS_RANGE_KEY = 'app_settings_insights_range';

export const InsightsRangeContext = createContext<InsightsRangeState | undefined>(undefined);

export const InsightsRangeProvider = ({ children, storage }: { children: React.ReactNode; storage: AppStorage }) => {
  const [periodCount, setPeriodCount] = useState<number>(() => InsightsRange.restore(storage.getString(APP_SETTINGS_INSIGHTS_RANGE_KEY)));

  useEffect(() => {
    try {
      storage.setString(APP_SETTINGS_INSIGHTS_RANGE_KEY, String(periodCount));
    } catch (error) {
      logger.error('Error persisting insights range preference:', error);
    }
  }, [periodCount, storage]);

  return (
    <InsightsRangeContext.Provider value={{ periodCount, options: InsightsRange.options, setPeriodCount }}>
      {children}
    </InsightsRangeContext.Provider>
  );
};
