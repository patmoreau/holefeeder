import { en } from '@holefeeder/shared/core';

export const translate = (key: string): string => {
  const value = key.split('.').reduce<unknown>((node, part) => (node as Record<string, unknown> | undefined)?.[part], en);
  return typeof value === 'string' ? value : key;
};
