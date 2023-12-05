import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { DecimalInputComponent } from './decimal-input.component';

describe('DecimalInputComponent', () => { // Fixed the parsing error by providing a string argument
  let component: DecimalInputComponent;
  let fixture: ComponentFixture<DecimalInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DecimalInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should render input element', () => {
    const inputElement = fixture.nativeElement.querySelector('input');
    expect(inputElement).toBeTruthy();
  });

  it('should bind control property to formControl input', () => {
    const formControl = new FormControl();
    component.control = formControl;
    fixture.detectChanges();
    expect(component.control).toBe(formControl);
  });

  it('should bind id property to id input', () => {
    const id = 'test-id';
    component.id = id;
    fixture.detectChanges();
    const inputElement = fixture.nativeElement.querySelector('input');
    expect(inputElement.id).toBe(id);
  });

  it('should bind placeholder property to placeholder input', () => {
    const placeholder = 'Enter a decimal number';
    component.placeholder = placeholder;
    fixture.detectChanges();
    const inputElement = fixture.nativeElement.querySelector('input');
    expect(inputElement.placeholder).toBe(placeholder);
  });

  it('should bind required property to required input', () => {
    component.required = true;
    fixture.detectChanges();
    const inputElement = fixture.nativeElement.querySelector('input');
    expect(inputElement.required).toBe(true);
  });
});
