import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { Settings, SettingsErrors } from '@/settings/core/settings';
import { DateIntervalTypeErrors, DateIntervalTypes } from '@/shared/core/date-interval-type';
import { DateOnlyErrors } from '@/shared/core/date-only';

describe('Settings', () => {
  it('parse valid settings JSON', () => {
    const jsonString: Record<string, unknown> = {
      effectiveDate: '2023-01-15',
      intervalType: 'monthly',
      frequency: 1,
    };

    const result = Settings.create(jsonString);

    expect(result).toBeSuccessWithValue({
      effectiveDate: '2023-01-15',
      intervalType: DateIntervalTypes.monthly,
      frequency: 1,
    });
  });

  it('fails for invalid intervalType', () => {
    const jsonString: Record<string, unknown> = {
      effectiveDate: '2023-01-15',
      intervalType: 'invalid',
      frequency: 1,
    };

    const result = Settings.create(jsonString);

    expect(result).toBeFailureWithErrors([DateIntervalTypeErrors.invalid]);
  });

  it('fails for missing effectiveDate', () => {
    const jsonString: Record<string, unknown> = {
      intervalType: 'monthly',
      frequency: 1,
    };

    const result = Settings.create(jsonString);

    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('fails for invalid frequency', () => {
    const jsonString: Record<string, unknown> = {
      effectiveDate: '2023-01-15',
      intervalType: 'monthly',
      frequency: 0,
    };

    const result = Settings.create(jsonString);

    expect(result).toBeFailureWithErrors([SettingsErrors.invalid]);
  });

  describe('toStoreItemData', () => {
    it('returns valid JSON', () => {
      const settings = aSettings();
      const result = Settings.toStoreItemData(settings);
      expect(result).toBe(JSON.stringify(settings));
    });
  });
});
