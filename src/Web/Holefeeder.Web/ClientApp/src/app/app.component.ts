import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ConsoleLogger } from '@app/core/logger';
import { AuthActions } from '@app/core/store/auth/auth.actions';
import { Store } from '@ngrx/store';
import { AppStore, CategoriesActions } from '@app/core/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
})
export class AppComponent implements OnInit {
  private store = inject(Store<AppStore>);
  private logger = inject(ConsoleLogger);

  constructor() {}

  ngOnInit() {
    this.store.dispatch(AuthActions.checkAuth());

    this.store
      .select(AuthFeature.selectIsAuthenticated)
      .subscribe(isAuthenticated => {
        if (isAuthenticated) {
          this.store.dispatch(CategoriesActions.loadCategories());
        } else {
          this.store.dispatch(CategoriesActions.clearCategories());
        }
      });
  }
}
