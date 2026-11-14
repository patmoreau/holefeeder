import { DateIntervalType, DateIntervalTypes } from './date-interval-type';
import { DateOnly } from './date-only';
import { Result } from './result';
import { Validate, Validator } from './validate';
import { today } from './with-date';

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

export const Settings = {
  create: create,
};
