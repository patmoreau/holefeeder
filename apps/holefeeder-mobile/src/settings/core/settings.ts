import { DateIntervalType, DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';
import { today } from '@/shared/core/with-date';

export const SETTINGS_CODE = 'settings';

export type Settings = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};

export const DefaultSettings: Settings = {
  effectiveDate: today(),
  intervalType: DateIntervalTypes.monthly,
  frequency: 1,
} as const;

export const SettingsErrors = {
  invalid: 'settings-invalid',
};

const isValidFrequency = Validator.number({ min: 1 });

const create = (value: Record<string, unknown>): Result<Settings> =>
  Result.combine<Settings>({
    effectiveDate: DateOnly.create(value.effectiveDate),
    intervalType: DateIntervalType.create(value.intervalType),
    frequency: Validate.validate(isValidFrequency, value.frequency, [SettingsErrors.invalid]),
  });

const toStoreItemData = (settings: Settings): string => JSON.stringify(settings);

export const Settings = {
  create: create,
  toStoreItemData: toStoreItemData,
};
