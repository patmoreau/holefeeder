import { Result } from '@/shared/core/result';
import { Validate, Validator } from '@/shared/core/validate';

export const AccountTypes = {
  checking: 'checking',
  creditCard: 'creditCard',
  creditLine: 'creditLine',
  investment: 'investment',
  loan: 'loan',
  mortgage: 'mortgage',
  savings: 'savings',
} as const;

export type AccountType = (typeof AccountTypes)[keyof typeof AccountTypes];

export const AccountTypeErrors = {
  invalid: 'account-type-invalid',
};

export const normalizeAccountType = (type: string): AccountType => {
  const normalized = type.trim().toLowerCase();
  switch (normalized) {
    case 'checking':
      return AccountTypes.checking;
    case 'creditcard':
    case 'credit_card':
      return AccountTypes.creditCard;
    case 'creditline':
    case 'credit_line':
      return AccountTypes.creditLine;
    case 'investment':
      return AccountTypes.investment;
    case 'loan':
      return AccountTypes.loan;
    case 'mortgage':
      return AccountTypes.mortgage;
    case 'savings':
      return AccountTypes.savings;
    default:
      return AccountTypes.checking;
  }
};

const isValid = Validator.enum<AccountType>({ values: AccountTypes });

const create = (value: unknown): Result<AccountType> => {
  let normalized = value;
  if (typeof value === 'string') {
    const potential = normalizeAccountType(value);
    if (potential !== AccountTypes.checking) {
      normalized = potential;
    } else if (value.trim().toLowerCase() === 'checking') {
      normalized = potential;
    }
  }

  const result = Validate.validate(isValid, normalized, [AccountTypeErrors.invalid]);
  if (result.isSuccess) {
    return Result.success(normalized as AccountType);
  }
  return result;
};

const valid = (value: unknown): AccountType => {
  return normalizeAccountType(value as string);
};

const multiplier = {
  [AccountTypes.checking]: 1,
  [AccountTypes.creditCard]: -1,
  [AccountTypes.creditLine]: -1,
  [AccountTypes.investment]: 1,
  [AccountTypes.loan]: -1,
  [AccountTypes.mortgage]: -1,
  [AccountTypes.savings]: 1,
};

export const AccountType = {
  create: create,
  valid: valid,
  multiplier: multiplier,
};
