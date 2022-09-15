import { IAccountInfo, ICategoryInfo } from '@app/shared';

export class Upcoming {
  constructor(
    public id: string,
    public date: Date,
    public amount: number,
    public description: string,
    public tags: string[],
    public category: ICategoryInfo,
    public account: IAccountInfo
  ) {}
}

