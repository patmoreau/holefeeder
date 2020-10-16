import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CashflowsListComponent } from './cashflows-list.component';

describe('CashflowsListComponent', () => {
  let component: CashflowsListComponent;
  let fixture: ComponentFixture<CashflowsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CashflowsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CashflowsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
