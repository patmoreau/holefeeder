export enum AccountType {
  checking = 'checking',
  creditCard = 'credit_card',
  creditLine = 'credit_line',
  investment = 'investment',
  loan = 'loan',
  mortgage = 'mortgage',
  savings = 'savings'
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
