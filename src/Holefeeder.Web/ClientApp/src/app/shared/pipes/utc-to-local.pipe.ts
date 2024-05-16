// utc-to-local.pipe.ts
import { Pipe, PipeTransform } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'utcToLocal',
  standalone: true,
})
export class UtcToLocalPipe implements PipeTransform {
  constructor(private datePipe: DatePipe) {}
  transform(utcDate: Date | string | number): string | null {
    // Convert UTC date to local date
    const localDate = dateFromUtc(utcDate);

    return this.datePipe.transform(localDate);
  }
}
