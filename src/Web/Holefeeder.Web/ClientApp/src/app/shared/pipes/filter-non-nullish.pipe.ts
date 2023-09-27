import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterNonNullish',
  standalone: true,
})
export class FilterNonNullishPipe implements PipeTransform {
  transform(value: any): any {
    if (Array.isArray(value)) {
      return value.filter(item => item !== null && item !== undefined);
    } else {
      return value;
    }
  }
}
