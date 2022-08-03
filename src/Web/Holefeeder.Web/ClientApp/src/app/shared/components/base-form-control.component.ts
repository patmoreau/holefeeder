import {
  ControlValueAccessor,
  FormControl,
  FormControlDirective,
  FormControlName,
  FormGroupDirective,
  NgControl,
  NgModel
} from "@angular/forms";
import {takeUntil} from "rxjs/operators";
import {Subject, tap} from "rxjs";
import {Component, Inject, Injector, OnDestroy, OnInit} from "@angular/core";

@Component({template: ""})
export abstract class BaseFormControlComponent<T> implements OnInit, ControlValueAccessor, OnDestroy {

  public control!: FormControl;

  public data!: T;

  protected readonly destroy = new Subject<void>();

  protected constructor(@Inject(Injector) protected injector: Injector) {
  }

  public ngOnInit(): void {
    this.setComponentControl();
  }

  public writeValue(value: T): void {
    this.data = value;
  }

  public onChange(): void {
    this.onChangeFn(this.data);
  }

  public registerOnChange(fn: (value: T | null) => T): void {
    this.onChangeFn = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouchFn = fn;
  }

  public onChangeFn = (value: T | null): T | null => value;

  public onTouchFn = (): void => {
  };

  public ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  private setComponentControl(): void {
    try {
      const formControl = this.injector.get(NgControl);

      switch (formControl.constructor) {
        case NgModel: {
          const {control, update} = formControl as NgModel;

          this.control = control;

          this.control.valueChanges
            .pipe(
              tap((value: T) => update.emit(value)),
              takeUntil(this.destroy),
            )
            .subscribe();
          break;
        }
        case FormControlName: {
          this.control = this.injector.get(FormGroupDirective).getControl(formControl as FormControlName);
          break;
        }
        default: {
          this.control = (formControl as FormControlDirective).form as FormControl;
          break;
        }
      }
    } catch (error) {
      this.control = new FormControl();
    }
  }
}
