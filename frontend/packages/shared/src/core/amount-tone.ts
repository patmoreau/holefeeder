export type AmountTone = 'positive' | 'negative' | 'neutral';

export type DisplayAmount = {
  amount: number;
  tone: AmountTone;
};

const roundsToZero = (value: number): boolean => Math.round(Math.abs(value) * 100) === 0;

const of = (value: number): AmountTone => {
  if (roundsToZero(value)) return 'neutral';
  return value > 0 ? 'positive' : 'negative';
};

const display = (value: number): DisplayAmount => {
  if (roundsToZero(value)) return { amount: 0, tone: 'neutral' };
  return { amount: Math.abs(value), tone: of(value) };
};

export const AmountTone = {
  roundsToZero: roundsToZero,
  of: of,
  display: display,
};
