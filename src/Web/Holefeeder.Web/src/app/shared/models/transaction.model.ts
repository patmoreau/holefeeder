import { ITransaction } from '../interfaces/transaction.interface';

export class Transaction implements ITransaction {
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
