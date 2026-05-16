import { aSaveSettingsForm } from '@/settings/core/save-settings/__tests__/save-settings-form-for-test';
import { SaveSettingsCommand, SaveSettingsCommandErrors } from '@/settings/core/save-settings/save-settings-command';
import { DateIntervalTypeErrors } from '@/shared/core/date-interval-type';
import { DateOnlyErrors } from '@/shared/core/date-only';

describe('SaveSettingsCommand', () => {
  it('succeeds with valid data', () => {
    const form = aSaveSettingsForm();
    const result = SaveSettingsCommand.create(form);
    expect(result).toBeSuccessWithValue({
      effectiveDate: form.effectiveDate,
      intervalType: form.intervalType,
      frequency: form.frequency,
    });
  });

  it('returns failure if effective date is invalid', () => {
    const form = aSaveSettingsForm({ effectiveDate: 'invalid-date' });
    const result = SaveSettingsCommand.create(form);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if interval type is invalid', () => {
    const form = aSaveSettingsForm({ intervalType: 'invalid' });
    const result = SaveSettingsCommand.create(form);
    expect(result).toBeFailureWithErrors([DateIntervalTypeErrors.invalid]);
  });

  it('returns failure if frequency is invalid', () => {
    const form = aSaveSettingsForm({ frequency: -1 });
    const result = SaveSettingsCommand.create(form);
    expect(result).toBeFailureWithErrors([SaveSettingsCommandErrors.invalidFrequency]);
  });
});
