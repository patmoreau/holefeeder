import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StatisticsCategoriesComponent } from './statistics-categories.component';

describe('StatisticsCategoryComponent', () => {
  let component: StatisticsCategoriesComponent;
  let fixture: ComponentFixture<StatisticsCategoriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StatisticsCategoriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StatisticsCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
