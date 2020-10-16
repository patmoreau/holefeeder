import { ICategoryInfo } from './category-info.interface';
import { IAccountInfo } from './account-info.interface';
import { ICashflowInfo } from './cashflow-info.interface';

export interface ITransactionDetail {
    id: string;
    date: Date;
    amount: number;
    description: string;
    category: ICategoryInfo;
    account: IAccountInfo;
    cashflow: ICashflowInfo;
    tags: string[];
}
