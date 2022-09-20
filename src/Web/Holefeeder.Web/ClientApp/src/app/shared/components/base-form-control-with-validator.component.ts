import { Component, Inject, Injector } from '@angular/core';
import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';
import { BaseFormControlComponent } from './base-form-control.component';

@Component({ template: '' })
export abstract class BaseFormControlWithValidatorComponent<T>
  extends BaseFormControlComponent<T>
  implements Validator
{
  protected constructor(@Inject(Injector) protected injector: Injector) {
    super(injector);
  }

  public ngOnInit(): void {
    super.ngOnInit();

    this.control.setValidators(
      this.control.validator
        ? [this.control.validator, this.validate]
        : this.validate
    );
  }

  public abstract validate(
    control: AbstractControl<any, any>
  ): ValidationErrors | null;

  public registerOnValidatorChange?(fn: () => void): void {
    this.onValidationChangeFn = fn;
  }

  public onValidationChangeFn: any = () => {};

  public onChange() {
    super.onChange();
    this.onValidationChangeFn();
  }
}
