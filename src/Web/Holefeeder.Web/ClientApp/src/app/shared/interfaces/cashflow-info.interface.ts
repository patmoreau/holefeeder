import { dateToUtc, dateFromUtc } from '../date-parser.helper';

export interface ICashflowInfo {
  id: string;
  date: Date;
}

export function cashflowInfoToServer(item: ICashflowInfo): ICashflowInfo {
  return Object.assign({} as ICashflowInfo, item, {
      date: dateToUtc(item.date)
  });
}

export function cashflowInfoFromServer(item: ICashflowInfo): ICashflowInfo {
  return Object.assign({} as ICashflowInfo, item, {
      date: dateFromUtc(item.date)
  });
}
