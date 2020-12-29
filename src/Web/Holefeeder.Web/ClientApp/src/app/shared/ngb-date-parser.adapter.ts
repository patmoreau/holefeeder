import { Injectable } from '@angular/core';
import { NgbDateAdapter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { startOfDay, startOfToday } from 'date-fns';

@Injectable()
export class NgbDateParserAdapter extends NgbDateAdapter<Date> {
  fromModel(date: Date): NgbDateStruct {
    return this._fromNativeDate(date ? date : startOfToday());
  }

  toModel(date: NgbDateStruct): Date {
    return date &&
      this.isInteger(date.year) &&
      this.isInteger(date.month) &&
      this.isInteger(date.day)
      ? this._toNativeDate(date)
      : null;
  }

  protected _fromNativeDate(date: Date): NgbDateStruct {
    return {
      year: date.getFullYear(),
      month: date.getMonth() + 1,
      day: date.getDate()
    };
  }

  protected _toNativeDate(date: NgbDateStruct): Date {
    const jsDate = new Date(date.year, date.month - 1, date.day, 0, 0, 0, 0);
    jsDate.setFullYear(date.year);
    return jsDate;
  }

  private isInteger(value: any): value is number {
    return (
      typeof value === 'number' &&
      isFinite(value) &&
      Math.floor(value) === value
    );
  }
}
