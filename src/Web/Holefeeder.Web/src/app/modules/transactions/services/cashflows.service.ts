import { Injectable } from "@angular/core";
import { MessageService } from "@app/core/services/message.service";
import { MessageType } from "@app/core/models/message-type.enum";
import { combineLatest, filter, Observable } from "rxjs";
import { TransactionsApiService } from "./api/transactions-api.service";
import { TransactionDetail } from "../models/transaction-detail.model";

@Injectable()
export class CashflowService {

  constructor(private apiService: TransactionsApiService, private messages: MessageService) {
  }

  findById(id: string): Observable<TransactionDetail | null> {
    return this.apiService.findOneById(id);
  }
}
