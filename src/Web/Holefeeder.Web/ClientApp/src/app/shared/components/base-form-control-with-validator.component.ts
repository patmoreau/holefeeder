import { Component, OnInit } from '@angular/core';
import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';
import { BaseFormControlComponent } from './base-form-control.component';

@Component({ template: '' })
export abstract class BaseFormControlWithValidatorComponent<T>
  extends BaseFormControlComponent<T>
  implements Validator, OnInit
{
  public override ngOnInit(): void {
    super.ngOnInit();

    this.control.setValidators(
      this.control.validator
        ? [this.control.validator, this.validate]
        : this.validate
    );
  }

  public abstract validate(
    control: AbstractControl<unknown, unknown>
  ): ValidationErrors | null;

  public registerOnValidatorChange?(fn: () => void): void {
    this.onValidationChangeFn = fn;
  }

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  public onValidationChangeFn = () => {};

  public override onChange() {
    super.onChange();
    this.onValidationChangeFn();
  }
}
