import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterNonNullish',
  standalone: true,
})
export class FilterNonNullishPipe<T> implements PipeTransform {
  transform(value: T | T[]): T | T[] {
    if (Array.isArray(value)) {
      return value.filter(item => item !== null && item !== undefined);
    } else {
      return value;
    }
  }
}
