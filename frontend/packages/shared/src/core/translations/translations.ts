import { en } from './locales/en-CA/translations';

type RecursiveRecord<T> = {
  [K in keyof T]: T[K] extends string ? string : RecursiveRecord<T[K]>;
};

export type TranslationStructure = RecursiveRecord<typeof en>;

type BuildLangObject<T, Path extends string = ''> = {
  [K in keyof T]: T[K] extends `${string}_${'one' | 'other' | 'zero' | 'few' | 'many'}`
    ? never
    : T[K] extends string
      ? Path extends ''
        ? K
        : `${Path}.${K & string}`
      : BuildLangObject<T[K], Path extends '' ? K & string : `${Path}.${K & string}`>;
};

function createLangObject<T extends Record<string, unknown>>(obj: T, prefix = ''): Record<string, unknown> {
  const result: Record<string, unknown> = {};
  const pluralSuffixRegex = /_(one|other|zero|few|many)$/;

  for (const key in obj) {
    if (pluralSuffixRegex.test(key)) {
      continue;
    }

    const path = prefix ? `${prefix}.${key}` : key;

    if (typeof obj[key] === 'string') {
      result[key] = path;
    } else {
      result[key] = createLangObject(obj[key] as Record<string, unknown>, path);
    }
  }

  return result;
}

export const tk = createLangObject(en) as BuildLangObject<typeof en>;
