import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared";

export class Series {
  constructor(
    public from: Date,
    public to: Date,
    public count: number,
    public amount: number
  ) {
  }
}

@Injectable({providedIn: "root"})
export class SeriesAdapter implements Adapter<Series> {
  adapt(item: any): Series {
    return new Series(new Date(item.from), new Date(item.to), item.count, item.amount);
  }
}
