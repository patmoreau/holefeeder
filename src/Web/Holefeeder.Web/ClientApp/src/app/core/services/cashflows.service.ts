import {Injectable} from "@angular/core";
import {MessageService} from "@app/core/services/message.service";
import {Observable} from "rxjs";
import {CashflowDetail} from "../models/cashflow-detail.model";
import {PagingInfo} from "@app/core/models/paging-info.model";
import {CashflowsApiService} from "./api/cashflows-api.service";

@Injectable({providedIn: 'root'})
export class CashflowsService {

  constructor(private apiService: CashflowsApiService, private messages: MessageService) {
  }


  find(offset: number, limit: number, sort: string[], filter: string[]): Observable<PagingInfo<CashflowDetail>> {
    return this.apiService.find(offset, limit, sort, filter);
  }

  findById(id: string): Observable<CashflowDetail | null> {
    return this.apiService.findOneById(id);
  }
}
