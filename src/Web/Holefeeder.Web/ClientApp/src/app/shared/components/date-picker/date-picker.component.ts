import {Component, Input} from '@angular/core';
import {NgbDateAdapter} from "@ng-bootstrap/ng-bootstrap";
import {NgbDateParserAdapter} from "@app/shared";
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from "@angular/forms";

@Component({
  selector: 'app-date-picker',
  templateUrl: './date-picker.component.html',
  styleUrls: ['./date-picker.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: DatePickerComponent,
    multi: true
  }, {provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class DatePickerComponent implements ControlValueAccessor {
  value: any;

  propagateChange = (_: any) => {
  };

  @Input() required: boolean = true;
  @Input() label: string = 'Date';

  writeValue(obj: any): void {
    this.value = obj || null;
  }

  registerOnChange(fn: any): void {
    this.propagateChange = fn;
  }

  registerOnTouched(fn: any): void {
  }

  update($event: any) {
    this.propagateChange($event);
  }
}
