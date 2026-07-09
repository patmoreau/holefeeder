export class SummaryValue {
  constructor(
    public gains: number,
    public expenses: number
  ) {}
}

export class Summary {
  constructor(
    public last: SummaryValue,
    public current: SummaryValue,
    public average: SummaryValue
  ) {}
}
