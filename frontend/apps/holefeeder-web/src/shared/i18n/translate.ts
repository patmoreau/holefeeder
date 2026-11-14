import { en, fr, LanguageType, type TranslationStructure } from '@holefeeder/shared/core';
import { Language } from './language';

const resources: Record<LanguageType, TranslationStructure> = { [LanguageType.en]: en, [LanguageType.fr]: fr };

const lookup = (key: string): unknown =>
  key.split('.').reduce<unknown>((node, part) => (node as Record<string, unknown> | undefined)?.[part], resources[Language.current()]);

const pluralOf = (key: string, count: unknown): unknown => {
  if (typeof count !== 'number') return undefined;
  return lookup(`${key}_${new Intl.PluralRules(Language.locale()).select(count)}`) ?? lookup(`${key}_other`);
};

export const translate = (key: string, values: Record<string, unknown> = {}): string => {
  const found = lookup(key);
  const value = typeof found === 'string' ? found : pluralOf(key, values.count);
  if (typeof value !== 'string') return key;
  return value.replace(/{{(\w+)}}/g, (placeholder, name: string) => (name in values ? String(values[name]) : placeholder));
};
