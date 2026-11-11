import { en } from '@holefeeder/shared/core';

export const translate = (key: string, values: Record<string, string> = {}): string => {
  const value = key.split('.').reduce<unknown>((node, part) => (node as Record<string, unknown> | undefined)?.[part], en);
  if (typeof value !== 'string') return key;
  return value.replace(/{{(\w+)}}/g, (placeholder, name: string) => values[name] ?? placeholder);
};
