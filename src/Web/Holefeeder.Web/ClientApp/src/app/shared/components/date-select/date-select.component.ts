import {Component, Input} from '@angular/core';
import {NgbDateAdapter} from "@ng-bootstrap/ng-bootstrap";
import {NgbDateParserAdapter} from "@app/shared/ngb-date-parser.adapter";
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from "@angular/forms";

@Component({
  selector: 'app-date-select',
  templateUrl: './date-select.component.html',
  styleUrls: ['./date-select.component.scss'],
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: DateSelectComponent,
    multi: true
  }, {provide: NgbDateAdapter, useClass: NgbDateParserAdapter}]
})
export class DateSelectComponent implements ControlValueAccessor {
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
