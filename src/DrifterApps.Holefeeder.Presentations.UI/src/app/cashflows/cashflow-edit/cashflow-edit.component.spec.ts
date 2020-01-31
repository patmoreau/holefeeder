import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashflowEditComponent } from './cashflow-edit.component';

describe('CashflowEditComponent', () => {
  let component: CashflowEditComponent;
  let fixture: ComponentFixture<CashflowEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashflowEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashflowEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
