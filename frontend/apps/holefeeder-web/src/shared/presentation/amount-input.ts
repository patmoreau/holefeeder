const amountPattern = /^\d+(\.\d{1,2})?$/;

export const isAmountInput = (value: string) => amountPattern.test(value);
