import { dateToUtc, dateFromUtc } from '../date-parser.helper';
import { Transaction } from '../models/transaction.model';

export interface ITransaction {
    id: string;
    date: Date;
    amount: number;
    description: string;
    account: string;
    category: string;
    cashflow: string;
    cashflowDate: Date;
    tags: string[];
}

export function transactionToServer(item: ITransaction): ITransaction {
    return Object.assign({} as ITransaction, item, {
        date: dateToUtc(item.date),
        cashflowDate: dateToUtc(item.cashflowDate)
    });
}

export function transactionFromServer(item: ITransaction): ITransaction {
    return Object.assign(new Transaction(), item, {
        date: dateFromUtc(item.date),
        cashflowDate: dateFromUtc(item.cashflowDate)
    });
}
