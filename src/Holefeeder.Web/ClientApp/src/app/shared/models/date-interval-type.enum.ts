export enum DateIntervalType {
  weekly = 'weekly',
  monthly = 'monthly',
  yearly = 'yearly',
  oneTime = 'onetime',
}

export const DateIntervalTypeNames = new Map<string, string>([
  [DateIntervalType.weekly, 'Weekly'],
  [DateIntervalType.monthly, 'Monthly'],
  [DateIntervalType.yearly, 'Yearly'],
  [DateIntervalType.oneTime, 'One time'],
]);
