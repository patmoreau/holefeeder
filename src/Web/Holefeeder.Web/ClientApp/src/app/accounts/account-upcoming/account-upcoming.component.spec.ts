import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountUpcomingComponent } from './account-upcoming.component';

describe('AccountUpcomingComponent', () => {
  let component: AccountUpcomingComponent;
  let fixture: ComponentFixture<AccountUpcomingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountUpcomingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountUpcomingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
