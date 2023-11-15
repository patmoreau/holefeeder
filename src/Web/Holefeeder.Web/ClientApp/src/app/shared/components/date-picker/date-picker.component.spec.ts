import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AbstractControl, FormsModule } from '@angular/forms';
import {
  NgbDateAdapter,
  NgbDateNativeAdapter,
  NgbDatepickerModule,
} from '@ng-bootstrap/ng-bootstrap';
import { DatePickerComponent } from './date-picker.component';

describe('DatePickerComponent', () => {
  let component: DatePickerComponent;
  let fixture: ComponentFixture<DatePickerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [FormsModule, NgbDatepickerModule],
      providers: [{ provide: NgbDateAdapter, useClass: NgbDateNativeAdapter }],
    });
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DatePickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set default properties', () => {
    expect(component.label).toEqual('Date');
    expect(component.placeholder).toEqual('yyyy-mm-dd');
    expect(component.required).toBeFalsy();
    expect(component.disabled).toBeFalsy();
  });

  it('should update value on date select', () => {
    const date = new Date('2023-11-02');
    component.onDateSelect(date);
    expect(component.value).toEqual(date);
  });

  it('should validate date', () => {
    let control = { value: null } as AbstractControl<unknown, unknown>; // Mocking AbstractControl
    expect(component.validate(control)).toBeNull();

    control = { value: new Date('invalid date') } as AbstractControl<
      unknown,
      unknown
    >; // Mocking AbstractControl
    const validationErrors = component.validate(control);
    expect(validationErrors).toEqual({ invalidDate: { value: control.value } });
  });
});
