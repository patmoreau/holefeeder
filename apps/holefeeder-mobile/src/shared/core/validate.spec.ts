import {
  ArrayValidatorErrors,
  BooleanValidatorErrors,
  EnumValidatorErrors,
  NumberValidatorErrors,
  PatternValidatorErrors,
  StringValidatorErrors,
  Validate,
  Validator,
} from '@/shared/core/validate';

const CUSTOM_ERROR = 'custom-error';

describe('Validate.validate', () => {
  const isValid = Validator.number({ min: 1, max: 10 });

  it('returns success for valid values', () => {
    const result = Validate.validate(isValid, 5);

    expect(result).toBeSuccessWithValue(5);
  });

  it('returns failure with error message from validator', () => {
    const result = Validate.validate(isValid, 10.5);

    expect(result).toBeFailureWithErrors([NumberValidatorErrors.max]);
  });

  it('allows overriding error message', () => {
    const isValidWithDefaults = Validator.number({ min: 1, max: 10 });
    const result = Validate.validate(isValidWithDefaults, 10.5, ['custom-error']);

    expect(result).toBeFailureWithErrors(['custom-error']);
  });
});

describe('Validator', () => {
  describe('booleanValidator', () => {
    const validator = Validator.boolean();

    it('returns success for boolean true', () => {
      expect(validator(true)).toBeSuccessWithValue(true);
    });

    it('returns success for boolean false', () => {
      expect(validator(false)).toBeSuccessWithValue(false);
    });

    it('returns failure for invalid type', () => {
      expect(validator('true')).toBeFailureWithErrors([BooleanValidatorErrors.type]);
    });
  });

  describe('enumValidator', () => {
    const TestEnums = {
      A: 'A',
      B: 'B',
    };
    type TestEnum = (typeof TestEnums)[keyof typeof TestEnums];

    const validator = Validator.enum<TestEnum>({ values: TestEnums });

    it('returns success for valid enum values', () => {
      expect(validator('A')).toBeSuccessWithValue('A');
      expect(validator('B')).toBeSuccessWithValue('B');
    });

    it('returns failure for invalid enum values', () => {
      expect(validator('C')).toBeFailureWithErrors([EnumValidatorErrors.invalid]);
    });

    it('allows overriding error message', () => {
      const customValidator = Validator.enum<TestEnum>({ values: TestEnums, errors: CUSTOM_ERROR });
      expect(customValidator('C')).toBeFailureWithErrors([CUSTOM_ERROR]);
      expect(customValidator(123)).toBeFailureWithErrors([CUSTOM_ERROR]);
    });
  });

  describe('numberValidator', () => {
    const validator = Validator.number({ min: 10, max: 20, integer: true });

    it('returns success for valid number', () => {
      expect(validator(15)).toBeSuccessWithValue(15);
    });

    it('returns failure for value less than min', () => {
      expect(validator(5)).toBeFailureWithErrors([NumberValidatorErrors.min]);
    });

    it('returns failure for value greater than max', () => {
      expect(validator(25)).toBeFailureWithErrors([NumberValidatorErrors.max]);
    });

    it('returns failure for invalid type', () => {
      expect(validator('15')).toBeFailureWithErrors([NumberValidatorErrors.type]);
    });

    it('returns failure for invalid integrer', () => {
      expect(validator(15.5)).toBeFailureWithErrors([NumberValidatorErrors.integer]);
    });

    it('allows overriding error messages', () => {
      const customValidator = Validator.number({
        min: 10,
        max: 20,
        integer: true,
        errors: { min: `min-${CUSTOM_ERROR}`, max: `max-${CUSTOM_ERROR}`, integer: `integer-${CUSTOM_ERROR}` },
      });
      expect(customValidator(5)).toBeFailureWithErrors([`min-${CUSTOM_ERROR}`]);
      expect(customValidator(25)).toBeFailureWithErrors([`max-${CUSTOM_ERROR}`]);
      expect(customValidator(15.5)).toBeFailureWithErrors([`integer-${CUSTOM_ERROR}`]);
    });
  });

  describe('patternValidator', () => {
    const pattern = /^[a-z]+$/;
    const validator = Validator.pattern({ pattern: pattern });

    it('returns success for matching pattern', () => {
      expect(validator('abc')).toBeSuccessWithValue('abc');
    });

    it('returns failure for non-matching pattern', () => {
      expect(validator('123')).toBeFailureWithErrors([PatternValidatorErrors.pattern]);
    });

    it('returns failure for invalid type', () => {
      expect(validator(123)).toBeFailureWithErrors([PatternValidatorErrors.type]);
    });

    it('allows overriding error messages', () => {
      const customValidator = Validator.pattern({ pattern: pattern, errors: CUSTOM_ERROR });
      expect(customValidator('123')).toBeFailureWithErrors([CUSTOM_ERROR]);
    });
  });

  describe('stringValidator', () => {
    const validator = Validator.string({ minLength: 3, maxLength: 5 });

    it('returns success for valid string length', () => {
      expect(validator('abcd')).toBeSuccessWithValue('abcd');
    });

    it('returns failure for string too short', () => {
      expect(validator('ab')).toBeFailureWithErrors([StringValidatorErrors.minLength]);
    });

    it('returns failure for string too long', () => {
      expect(validator('abcdef')).toBeFailureWithErrors([StringValidatorErrors.maxLength]);
    });

    it('returns failure for invalid type', () => {
      expect(validator(123)).toBeFailureWithErrors([StringValidatorErrors.type]);
    });

    it('allows overriding error messages', () => {
      const customValidator = Validator.string({
        minLength: 3,
        maxLength: 5,
        errors: { minLength: `min-${CUSTOM_ERROR}`, maxLength: `max-${CUSTOM_ERROR}` },
      });
      expect(customValidator('ab')).toBeFailureWithErrors([`min-${CUSTOM_ERROR}`]);
      expect(customValidator('abcdef')).toBeFailureWithErrors([`max-${CUSTOM_ERROR}`]);
    });
  });

  describe('arrayValidator', () => {
    const itemValidator = Validator.string({ minLength: 2 });
    const validator = Validator.array(itemValidator, { minLength: 1, maxLength: 2 });

    it('returns success for valid array', () => {
      expect(validator(['abc', 'def'])).toBeSuccessWithValue(['abc', 'def']);
    });

    it('fails on min length error', () => {
      expect(validator([])).toBeFailureWithErrors([ArrayValidatorErrors.minLength]);
    });

    it('fails on max length error', () => {
      expect(validator(['abc', 'def', 'ghi'])).toBeFailureWithErrors([ArrayValidatorErrors.maxLength]);
    });

    it('returns failure for invalid type', () => {
      expect(validator('not-an-array')).toBeFailureWithErrors([ArrayValidatorErrors.type]);
    });

    it('bubbles up item error when no override provided', () => {
      expect(validator(['a', 'def'])).toBeFailureWithErrors([`index-0-${StringValidatorErrors.minLength}`]);
    });

    it('allows overriding error messages', () => {
      const customValidator = Validator.array(itemValidator, {
        minLength: 1,
        maxLength: 2,
        errors: { minLength: `min-${CUSTOM_ERROR}`, maxLength: `max-${CUSTOM_ERROR}` },
      });
      expect(customValidator([])).toBeFailureWithErrors([`min-${CUSTOM_ERROR}`]);
      expect(customValidator(['abc', 'def', 'ghi'])).toBeFailureWithErrors([`max-${CUSTOM_ERROR}`]);
    });
  });
});
