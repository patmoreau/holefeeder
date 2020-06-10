import { Component, OnInit, forwardRef, Input } from '@angular/core';
import {
  NG_VALUE_ACCESSOR,
  ControlValueAccessor,
  FormBuilder,
  FormControl,
  Validators
} from '@angular/forms';
import { BaseControlValueAccessor } from '@app/shared/base-control-value.accessor';

@Component({
  selector: 'dfta-currency',
  templateUrl: './currency.component.html',
  styleUrls: ['./currency.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CurrencyComponent),
      multi: true
    }
  ]
})
export class CurrencyComponent extends BaseControlValueAccessor<number>
  implements OnInit, ControlValueAccessor {
  currency: FormControl;

  constructor(private formBuilder: FormBuilder) {
    super();

    this.currency = this.formBuilder.control(null, Validators.required);

    this.currency.valueChanges.subscribe((val: number) => {
      this.value = val;
      this.onChange(val);
    });
  }

  ngOnInit() {}

  writeValue(val: number): void {
    super.writeValue(val);
    this.currency.patchValue(val);
  }
}
