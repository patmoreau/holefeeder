import { useState } from 'react';

const storageKey = 'holefeeder.insights.periods';
const offered = [3, 6, 12];
const fallback = 6;

const stored = (): number => {
  try {
    const value = Number(localStorage.getItem(storageKey));
    return offered.includes(value) ? value : fallback;
  } catch {
    return fallback;
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
