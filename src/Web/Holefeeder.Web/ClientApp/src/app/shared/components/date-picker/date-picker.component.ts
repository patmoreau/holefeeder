import { Component, forwardRef, Inject, Injector, Input } from '@angular/core';
import {
  AbstractControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
} from '@angular/forms';
import { BaseFormControlWithValidatorComponent } from '@app/shared/components/base-form-control-with-validator.component';
import {
  NgbDateAdapter,
  NgbDateNativeAdapter,
} from '@ng-bootstrap/ng-bootstrap';
import { isValid, startOfToday } from 'date-fns';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      useExisting: forwardRef(() => DatePickerComponent),
      multi: true,
    },
    { provide: NgbDateAdapter, useClass: NgbDateNativeAdapter },
  ],
})
export class DatePickerComponent extends BaseFormControlWithValidatorComponent<Date> {
  @Input()
  public label = 'Date';

  @Input()
  public placeholder = 'yyyy-mm-dd';

  @Input()
  public required = false;

  @Input()
  public disabled = false;

  constructor(@Inject(Injector) protected injector: Injector) {
    super(injector);

    this.data = startOfToday();
  }

  public override validate(
    control: AbstractControl<any, any>
  ): ValidationErrors | null {
    if (control.value === null) {
      return null;
    }
    const valid = isValid(control.value);
    return valid ? null : { invalidDate: { value: control.value } };
  }
}
