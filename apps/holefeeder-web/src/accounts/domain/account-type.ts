import { Result, Validate, Validator } from '@holefeeder/core';

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
  const normalized = typeof value === 'string' ? normalizeAccountType(value) : value;
  return Validate.validate(isValid, normalized, [AccountTypeErrors.invalid]);
};

const valid = (value: unknown): AccountType => normalizeAccountType(value as string);

export const AccountType = { create, valid };
