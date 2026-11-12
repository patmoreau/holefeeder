export type Tag = {
  tag: string;
  transactionCount: number;
  activeCashflowCount: number;
};

const valid = (value: Record<string, unknown>): Tag => ({
  tag: value.tag as string,
  transactionCount: value.transactionCount as number,
  activeCashflowCount: value.activeCashflowCount as number,
});

export const Tag = {
  valid: valid,
};
