export class PagingInfo<T> {
  constructor(public totalCount: number, public items: T[]) {}
}
