import { describe, expect, it } from '@jest/globals';
import { DateOnly } from '@/shared/core/date-only';
import { SettingsFormData } from '../../core/settings-form-data';
import { SettingsFormError, validateSettingsForm } from './use-settings-form';

describe('SettingsFormError', () => {
  it('should have effectiveDateRequired error constant', () => {
    expect(SettingsFormError.effectiveDateRequired).toBe('effectiveDateRequired');
  });

  it('should have frequencyRequired error constant', () => {
    expect(SettingsFormError.frequencyRequired).toBe('frequencyRequired');
  });
});

describe('validateSettingsForm', () => {
  describe('valid form data', () => {
    it('should return no errors for valid form data', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 1,
      };

      const errors = validateSettingsForm(formData);

      expect(errors).toEqual({});
      expect(Object.keys(errors).length).toBe(0);
    });

    it('should return no errors for valid form data with high frequency', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-12-31'),
        intervalType: 'weekly',
        frequency: 52,
      };

      const errors = validateSettingsForm(formData);

      expect(errors).toEqual({});
    });

    it('should return no errors for valid form data with different intervalType', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2024-06-15'),
        intervalType: 'yearly',
        frequency: 5,
      };

      const errors = validateSettingsForm(formData);

      expect(errors).toEqual({});
    });
  });

  describe('effectiveDate validation', () => {
    it('should return effectiveDateRequired error when effectiveDate is empty string', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid(''),
        intervalType: 'monthly',
        frequency: 1,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.effectiveDate).toBe(SettingsFormError.effectiveDateRequired);
      expect(Object.keys(errors).length).toBe(1);
    });

    it('should not return error for non-empty effectiveDate', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-01'),
        intervalType: 'monthly',
        frequency: 1,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.effectiveDate).toBeUndefined();
    });
  });

  describe('frequency validation', () => {
    it('should return frequencyRequired error when frequency is 0', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 0,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBe(SettingsFormError.frequencyRequired);
      expect(Object.keys(errors).length).toBe(1);
    });

    it('should return frequencyRequired error when frequency is negative', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: -1,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBe(SettingsFormError.frequencyRequired);
    });

    it('should return frequencyRequired error when frequency is large negative number', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: -100,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBe(SettingsFormError.frequencyRequired);
    });

    it('should not return error for positive frequency', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 1,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBeUndefined();
    });

    it('should not return error for large positive frequency', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 999,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBeUndefined();
    });
  });

  describe('multiple validation errors', () => {
    it('should return both errors when effectiveDate is empty and frequency is 0', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid(''),
        intervalType: 'monthly',
        frequency: 0,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.effectiveDate).toBe(SettingsFormError.effectiveDateRequired);
      expect(errors.frequency).toBe(SettingsFormError.frequencyRequired);
      expect(Object.keys(errors).length).toBe(2);
    });

    it('should return both errors when effectiveDate is empty and frequency is negative', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid(''),
        intervalType: 'weekly',
        frequency: -5,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.effectiveDate).toBe(SettingsFormError.effectiveDateRequired);
      expect(errors.frequency).toBe(SettingsFormError.frequencyRequired);
      expect(Object.keys(errors).length).toBe(2);
    });
  });

  describe('edge cases', () => {
    it('should handle form data with decimal frequency (boundary test)', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 0.5,
      };

      const errors = validateSettingsForm(formData);

      // Should be valid since 0.5 > 0
      expect(errors.frequency).toBeUndefined();
    });

    it('should handle form data with very small positive frequency', () => {
      const formData: SettingsFormData = {
        effectiveDate: DateOnly.valid('2023-01-15'),
        intervalType: 'monthly',
        frequency: 0.001,
      };

      const errors = validateSettingsForm(formData);

      expect(errors.frequency).toBeUndefined();
    });

    it('should handle all valid intervalType values', () => {
      const intervalTypes = ['weekly', 'monthly', 'yearly', 'oneTime'] as const;

      intervalTypes.forEach((intervalType) => {
        const formData: SettingsFormData = {
          effectiveDate: DateOnly.valid('2023-01-15'),
          intervalType,
          frequency: 1,
        };

        const errors = validateSettingsForm(formData);

        expect(errors).toEqual({});
      });
    });
  });
});
