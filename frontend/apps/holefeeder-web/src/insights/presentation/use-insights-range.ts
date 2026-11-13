import { InsightsRange } from '@holefeeder/shared/core';
import { useState } from 'react';

const storageKey = 'holefeeder.insights.periods';

const stored = (): number => {
  try {
    return InsightsRange.restore(localStorage.getItem(storageKey));
  } catch {
    return InsightsRange.fallback;
  }
};

const remember = (periods: number) => {
  try {
    localStorage.setItem(storageKey, String(periods));
  } catch {
    return;
  }
};

export const useInsightsRange = () => {
  const [periods, setPeriods] = useState(stored);

  const choose = (value: number) => {
    remember(value);
    setPeriods(value);
  };

  return { periods: periods, choose: choose };
};
