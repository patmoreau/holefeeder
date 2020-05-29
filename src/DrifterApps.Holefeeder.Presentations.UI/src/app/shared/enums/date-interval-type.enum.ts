export enum DateIntervalType {
  weekly = 'Weekly',
  monthly = 'Monthly',
  yearly = 'Yearly',
  oneTime = 'OneTime'
}

export const DateIntervalTypeNames = new Map<string, string>([
  [DateIntervalType.weekly, 'Weekly'],
  [DateIntervalType.monthly, 'Monthly'],
  [DateIntervalType.yearly, 'Yearly'],
  [DateIntervalType.oneTime, 'One time']
]);
