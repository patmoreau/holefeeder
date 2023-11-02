import { CommonModule } from '@angular/common';
import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  forwardRef,
  Input,
} from '@angular/core';
import {
  AbstractControl,
  FormsModule,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
} from '@angular/forms';
import {
  NgbDateAdapter,
  NgbDateNativeAdapter,
  NgbDatepickerModule,
} from '@ng-bootstrap/ng-bootstrap';
import { isValid, startOfToday } from 'date-fns';
import { BaseFormControlWithValidatorComponent } from '../base-form-control-with-validator.component';

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
  standalone: true,
  imports: [CommonModule, FormsModule, NgbDatepickerModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
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

  constructor() {
    super();
    this.value = startOfToday();
  }

  public override writeValue(value: Date): void {
    this.value = value;
  }

  public onDateSelect(value: Date) {
    this.value = value;
    this.onChange();
  }

  public override validate(
    control: AbstractControl<unknown, unknown>
  ): ValidationErrors | null {
    if (control.value === null) {
      return null;
    }
    const valid = isValid(control.value);
    return valid ? null : { invalidDate: { value: control.value } };
  }
}
