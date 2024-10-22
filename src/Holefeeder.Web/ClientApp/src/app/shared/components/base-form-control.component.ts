import { Component, inject, Injector, OnDestroy, OnInit } from '@angular/core';
import {
  ControlValueAccessor,
  FormControl,
  FormControlDirective,
  FormControlName,
  FormGroupDirective,
  NgControl,
  NgModel,
} from '@angular/forms';
import { Subject, tap } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({ template: '' })
export abstract class BaseFormControlComponent<T>
  implements OnInit, ControlValueAccessor, OnDestroy
{
  public control!: FormControl;

  public value!: T | null;

  protected readonly destroy = new Subject<void>();

  protected injector: Injector = inject(Injector);

  public ngOnInit(): void {
    this.setComponentControl();
  }

  public writeValue(value: T): void {
    this.value = value;
  }

  public onChange(): void {
    this.onChanged(this.value);
  }

  public registerOnChange(fn: (value: T | null) => T): void {
    this.onChanged = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  public onChanged = (value: T | null): T | null => value;

  // eslint-disable-next-line @typescript-eslint/no-empty-function
  public onTouched = (): void => {};

  public ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  private setComponentControl(): void {
    try {
      const formControl = this.injector.get(NgControl);

      switch (formControl.constructor) {
        case NgModel: {
          const { control, update } = formControl as NgModel;

          this.control = control;

          this.control.valueChanges
            .pipe(
              tap((value: T) => update.emit(value)),
              takeUntil(this.destroy)
            )
            .subscribe();
          break;
        }
        case FormControlName: {
          this.control = this.injector
            .get(FormGroupDirective)
            .getControl(formControl as FormControlName);
          break;
        }
        default: {
          this.control = (formControl as FormControlDirective)
            .form as FormControl;
          break;
        }
      }
    } catch {
      this.control = new FormControl();
    }
  }
}
