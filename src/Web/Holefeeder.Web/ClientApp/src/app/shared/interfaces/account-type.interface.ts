export function accountTypeMultiplier(type: string): number {
  switch (type) {
    case 'Checking':
    case 'Investment':
    case 'Savings':
      return 1;
    default:
      return -1;
  }
}
