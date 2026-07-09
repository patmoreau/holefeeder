import { formatDate } from '@angular/common';

export function dateFromUtc(date: Date | string | number): Date {
  const utc = new Date(date);
  return new Date(utc.getUTCFullYear(), utc.getUTCMonth(), utc.getUTCDate());
}

export function dateToUtc(date: Date | string | number): Date {
  const utc = new Date(date);
  utc.setUTCHours(0);
  utc.setUTCMinutes(0);
  utc.setUTCSeconds(0);
  utc.setUTCMilliseconds(0);
  return utc;
}

export function dateToDateOnly(date: Date | string | number): string {
  const utc = dateFromUtc(date);
  return formatDate(utc, 'yyyy-MM-dd', 'en-US', undefined);
}

const dateOnlyFormat = /^\d{4}-\d{2}-\d{2}$/;

export function isDateOnlyString(value: unknown): boolean {
  if (value === null || value === undefined) {
    return false;
  }
  if (typeof value === 'string') {
    return dateOnlyFormat.test(value);
  }
  return false;
}
