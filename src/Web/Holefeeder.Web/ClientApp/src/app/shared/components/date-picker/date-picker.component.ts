import {Component, forwardRef, Input, Optional, Self} from '@angular/core';
import {NgbDateAdapter, NgbDateNativeAdapter} from "@ng-bootstrap/ng-bootstrap";
import {ControlValueAccessor, NgControl, NG_VALIDATORS} from "@angular/forms";
import {startOfToday} from "date-fns";
import {DateValidator} from "@app/shared";

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  providers: [
    {provide: NG_VALIDATORS, useExisting: forwardRef(() => DateValidator), multi: true},
    {provide: NgbDateAdapter, useClass: NgbDateNativeAdapter}
  ]
})
export class DatePickerComponent implements ControlValueAccessor {
  @Input()
  public label = 'Date';

  @Input()
  public placeholder = 'yyyy-mm-dd';

  @Input()
  public required = false;

  @Input()
  public disabled = false;

  @Input()
  public data: Date = startOfToday();

  private errorMessages = new Map<string, () => string>();

  public onChangeFn = (_: any) => {
  };

  public onTouchedFn = () => {
  };

  constructor(@Self() @Optional() public control: NgControl) {
    this.control && (this.control.valueAccessor = this);
    this.errorMessages.set('required', () => `${this.label} is required.`);
    this.errorMessages.set('invalidDate', () => `${this.label} is invalid.`);
  }

  public get invalid(): boolean {
    return this.control?.invalid ?? false;
  }

  public get showError(): boolean {
    if (!this.control) {
      return false;
    }

    const {dirty, touched} = this.control;

    return this.invalid ? ((dirty || touched) ?? false) : false;
  }

  public get errors(): Array<string> {
    if (!this.control) {
      return [];
    }

    const {errors} = this.control;
    return Object.keys(errors!).map(key => this.errorMessages.has(key) ? this.errorMessages.get(key)!() : <string>errors![key] || key);
  }

  public registerOnChange(fn: any): void {
    this.onChangeFn = fn;
  }

  public registerOnTouched(fn: any): void {
    this.onTouchedFn = fn;
  }

  public setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  public writeValue(obj: any): void {
    this.data = obj || null;
  }

  public onChange() {
    this.onChangeFn(this.data);
  }
}
