import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DateViewComponent } from './date-view.component';

describe('DateViewComponent', () => {
  let component: DateViewComponent;
  let fixture: ComponentFixture<DateViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
    declarations: [DateViewComponent],
    teardown: { destroyAfterEach: false }
})
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DateViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
