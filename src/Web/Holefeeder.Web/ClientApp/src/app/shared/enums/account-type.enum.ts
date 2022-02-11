export enum AccountType {
  checking = 'Checking',
  creditCard = 'CreditCard',
  creditLine = 'CreditLine',
  investment = 'Investment',
  loan = 'Loan',
  mortgage = 'Mortgage',
  savings = 'Savings'
}

export const AccountTypeNames = new Map<string, string>([
  [AccountType.checking, 'Checking'],
  [AccountType.creditCard, 'Credit card'],
  [AccountType.creditLine, 'Credit line'],
  [AccountType.investment, 'Investment'],
  [AccountType.loan, 'Loan'],
  [AccountType.mortgage, 'Mortgage'],
  [AccountType.savings, 'Savings']
]);

export function accountTypeMultiplier(type: AccountType): number {
  switch (type) {
    case AccountType.checking:
    case AccountType.investment:
    case AccountType.savings:
      return 1;
    default:
      return -1;
  }
}
