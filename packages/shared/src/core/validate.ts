import { Result } from './result';

type Validator<T> = (value: unknown) => Result<T>;

const validate = <T>(validator: Validator<T>, value: unknown, errors?: string[]): Result<T> => {
  const result = validator(value);
  return result.isFailure ? Result.failure(errors ?? result.errors) : result;
};

export const Validate = { validate: validate } as const;

type EnumOptions<T> = {
  values: Record<string, T>;
  errors?: string;
};

export const EnumValidatorErrors = {
  invalid: 'enum-invalid',
};

const enumValidator =
  <T extends string>(opts: EnumOptions<T>): Validator<T> =>
  (value) => {
    const allowedValues = Object.values(opts.values);

    if (typeof value !== 'string' || !allowedValues.includes(value as T)) {
      return Result.failure([opts.errors ?? EnumValidatorErrors.invalid]);
    }
    return Result.success(value as T);
  };

type NumberOptions = {
  min?: number;
  max?: number;
  integer?: boolean;
  errors?: Partial<{ min: string; max: string; integer: string }>;
};

export const NumberValidatorErrors = {
  min: 'number-min',
  max: 'number-max',
  integer: 'number-integer',
  type: 'number-type',
};

const numberValidator =
  <T extends number>(opts: NumberOptions = {}): Validator<T> =>
  (value: unknown) => {
    if (typeof value !== 'number' || isNaN(value)) return Result.failure([NumberValidatorErrors.type]);
    if (opts.integer && !Number.isInteger(value)) return Result.failure([opts.errors?.integer ?? NumberValidatorErrors.integer]);
    if (opts.min !== undefined && value < opts.min) return Result.failure([opts.errors?.min ?? NumberValidatorErrors.min]);
    if (opts.max !== undefined && value > opts.max) return Result.failure([opts.errors?.max ?? NumberValidatorErrors.max]);
    return Result.success(value as T);
  };

type PatternOptions = {
  pattern: RegExp;
  errors?: string;
};

export const PatternValidatorErrors = {
  pattern: 'pattern-invalid',
  type: 'pattern-type',
};

const patternValidator =
  <T extends string>(opts: PatternOptions): Validator<T> =>
  (value) => {
    if (typeof value !== 'string') return Result.failure([PatternValidatorErrors.type]);
    if (!opts.pattern.test(value)) return Result.failure([opts.errors ?? PatternValidatorErrors.pattern]);
    return Result.success(value as T);
  };

type StringOptions = {
  minLength?: number;
  maxLength?: number;
  errors?: Partial<{ minLength: string; maxLength: string }>;
};

export const StringValidatorErrors = {
  minLength: 'string-min-length',
  maxLength: 'string-max-length',
  type: 'string-type',
};

const stringValidator =
  <T extends string>(opts: StringOptions = {}): Validator<T> =>
  (value) => {
    if (typeof value !== 'string') return Result.failure([StringValidatorErrors.type]);
    if (opts.minLength !== undefined && value.length < opts.minLength)
      return Result.failure([opts.errors?.minLength ?? StringValidatorErrors.minLength]);
    if (opts.maxLength !== undefined && value.length > opts.maxLength)
      return Result.failure([opts.errors?.maxLength ?? StringValidatorErrors.maxLength]);
    return Result.success(value as T);
  };

type ArrayOptions = {
  minLength?: number;
  maxLength?: number;
  errors?: Partial<{
    minLength: string;
    maxLength: string;
  }>;
};

export const ArrayValidatorErrors = {
  minLength: 'array-min-length',
  maxLength: 'array-max-length',
  items: 'array-items',
  type: 'array-type',
};

const arrayValidator =
  <T>(itemValidator: Validator<T>, opts: ArrayOptions = {}): Validator<T[]> =>
  (value) => {
    if (!Array.isArray(value)) return Result.failure([ArrayValidatorErrors.type]);
    if (opts.minLength !== undefined && value.length < opts.minLength)
      return Result.failure([opts.errors?.minLength ?? ArrayValidatorErrors.minLength]);
    if (opts.maxLength !== undefined && value.length > opts.maxLength)
      return Result.failure([opts.errors?.maxLength ?? ArrayValidatorErrors.maxLength]);

    const errors = value.flatMap((item, i) => {
      const result = itemValidator(item);
      if (result.isSuccess) return [];
      return result.errors.map((e) => `index-${i}-${e}`);
    });

    return errors.length > 0 ? Result.failure(errors) : Result.success(value as T[]);
  };

export const Validator = {
  enum: enumValidator,
  number: numberValidator,
  pattern: patternValidator,
  string: stringValidator,
  array: arrayValidator,
} as const;
