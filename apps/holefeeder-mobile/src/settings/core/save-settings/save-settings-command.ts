import { DateIntervalType } from '@/shared/core/date-interval-type';
import { DateOnly } from '@/shared/core/date-only';
import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export type SaveSettingsCommand = {
  effectiveDate: DateOnly;
  intervalType: DateIntervalType;
  frequency: number;
};

export const SaveSettingsCommandErrors = {
  invalidFrequency: 'invalid-frequency',
};

const isValidFrequency = Validator.number({ min: 1 });

const create = (command: Record<string, unknown>): Result<SaveSettingsCommand> =>
  Result.combine<SaveSettingsCommand>({
    effectiveDate: DateOnly.create(command.effectiveDate),
    intervalType: DateIntervalType.create(command.intervalType),
    frequency: Validate.validate(isValidFrequency, command.frequency, [SaveSettingsCommandErrors.invalidFrequency]),
  });

export const SaveSettingsCommand = {
  create: create,
};
