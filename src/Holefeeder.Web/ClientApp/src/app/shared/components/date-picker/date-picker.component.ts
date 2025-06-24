
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
import { LoggerService } from '@app/core/logger';
import { dateFromUtc, dateToUtc } from '@app/shared/helpers';

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  standalone: true,
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
  imports: [FormsModule, NgbDatepickerModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
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

  constructor(private logger: LoggerService) {
    super();
    this.value = startOfToday();
  }

  public override writeValue(value: Date): void {
    this.logger.info('writeValue', value);
    this.logger.info('writeValue - utc', dateFromUtc(value));
    this.value = value;
  }

  public onDateSelect(value: Date) {
    this.logger.info('onDateSelect', value);
    this.logger.info('onDateSelect - utc', dateToUtc(value));
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
