import { en } from './locales/en-CA/translations';

type RecursiveRecord<T> = {
  [K in keyof T]: T[K] extends string ? string : RecursiveRecord<T[K]>;
};

export type TranslationStructure = RecursiveRecord<typeof en>;

type PluralSuffix = 'one' | 'other' | 'zero' | 'few' | 'many';

type BaseKey<K> = K extends `${infer Base}_${PluralSuffix}` ? Base : K;

type BuildLangObject<T, Path extends string = ''> = {
  [K in keyof T as BaseKey<K>]: T[K] extends string
    ? Path extends ''
      ? BaseKey<K>
      : `${Path}.${BaseKey<K> & string}`
    : BuildLangObject<T[K], Path extends '' ? K & string : `${Path}.${K & string}`>;
};

function createLangObject<T extends Record<string, unknown>>(obj: T, prefix = ''): Record<string, unknown> {
  const result: Record<string, unknown> = {};
  const pluralSuffixRegex = /_(one|other|zero|few|many)$/;

  for (const key in obj) {
    const name = key.replace(pluralSuffixRegex, '');
    const path = prefix ? `${prefix}.${name}` : name;

    if (typeof obj[key] === 'string') {
      result[name] = path;
    } else {
      result[name] = createLangObject(obj[key] as Record<string, unknown>, path);
    }
  }

  return result;
}

export const tk = createLangObject(en) as BuildLangObject<typeof en>;
