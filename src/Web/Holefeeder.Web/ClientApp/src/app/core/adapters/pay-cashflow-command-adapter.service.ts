import { Injectable } from '@angular/core';
import { dateToUtc } from '@app/shared/helpers';
import { Adapter, PayCashflowCommand } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class PayCashflowCommandAdapter implements Adapter<PayCashflowCommand> {
  adapt(item: {
    date: Date;
    amount: number;
    cashflow: string;
    cashflowDate: Date;
  }): PayCashflowCommand {
    return new PayCashflowCommand(
      dateToUtc(item.date),
      item.amount,
      item.cashflow,
      dateToUtc(item.cashflowDate)
    );
  }
}
