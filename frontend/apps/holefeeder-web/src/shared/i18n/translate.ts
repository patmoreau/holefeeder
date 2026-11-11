import { en, fr, LanguageType, type TranslationStructure } from '@holefeeder/shared/core';
import { Language } from './language';

const resources: Record<LanguageType, TranslationStructure> = { [LanguageType.en]: en, [LanguageType.fr]: fr };

export const translate = (key: string, values: Record<string, unknown> = {}): string => {
  const value = key
    .split('.')
    .reduce<unknown>((node, part) => (node as Record<string, unknown> | undefined)?.[part], resources[Language.current()]);
  if (typeof value !== 'string') return key;
  return value.replace(/{{(\w+)}}/g, (placeholder, name: string) => (name in values ? String(values[name]) : placeholder));
};
