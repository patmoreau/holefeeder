import { Directive, Injectable } from '@angular/core';
import {
  AbstractControl,
  NG_VALIDATORS,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { isValid } from 'date-fns';

@Injectable()
export class DateValidator implements Validator {
  validate(control: AbstractControl<any, any>): ValidationErrors | null {
    const invalid = isValid(control.value);
    return invalid ? { invalidDate: { value: control.value } } : null;
  }
}

@Directive({
  selector: '[appValidDate]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useExisting: DateValidatorDirective,
      multi: true,
    },
  ],
})
export class DateValidatorDirective implements Validator {
  constructor(private validator: DateValidator) {}

  validate(control: AbstractControl<any, any>): ValidationErrors | null {
    return this.validator.validate(control);
  }
}
