// utc-to-local.pipe.ts
import { Pipe, PipeTransform, inject } from '@angular/core';
import { dateFromUtc } from '@app/shared/helpers';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'utcToLocal',
  standalone: true,
})
export class UtcToLocalPipe implements PipeTransform {
  private datePipe = inject(DatePipe);

  transform(utcDate: Date | string | number): string | null {
    // Convert UTC date to local date
    const localDate = dateFromUtc(utcDate);

    return this.datePipe.transform(localDate);
  }
}
