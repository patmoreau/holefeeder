const options = [3, 6, 12];
const fallback = 6;

const restore = (stored: string | null | undefined): number => {
  const value = Number(stored);
  return options.includes(value) ? value : fallback;
};

export const InsightsRange = {
  options: options,
  fallback: fallback,
  restore: restore,
};
