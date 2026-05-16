import { Result } from './result';
import { Validate, Validator } from './validate';

export type Money = number & { readonly __brand: 'Money' };

export const MoneyErrors = {
  invalid: 'money-invalid',
};

const isValidMoney = Validator.number<Money>({ min: 0 });

const create = (value: unknown): Result<Money> => {
  const moneyResult = Validate.validate<Money>(isValidMoney, value, [MoneyErrors.invalid]);
  if (!moneyResult.isSuccess) return moneyResult;

  const money = toCents(moneyResult.value);

  return Result.success(fromCents(money));
};

const valid = (value: unknown): Money => value as Money;

const toCents = (money: Money): number => Math.round(money * 100);

const fromCents = (cents: number): Money => {
  const value = cents / 100;
  return (Math.round(value * 100) / 100) as Money;
};

const ZERO: Money = 0 as Money;

const sum = (...values: Money[]): Money => {
  const totalCents = values.reduce((acc, curr) => acc + toCents(curr), 0);
  return fromCents(totalCents);
};

const subtract = (a: Money, b: Money): Money => {
  return fromCents(toCents(a) - toCents(b));
};

const multiply = (value: Money, factor: number): Money => {
  const resultCents = Math.round(toCents(value) * factor);
  return fromCents(resultCents);
};

export const Money = {
  create: create,
  valid: valid,
  toCents: toCents,
  fromCents: fromCents,
  ZERO: ZERO,
  sum: sum,
  subtract: subtract,
  multiply: multiply,
} as const;
