import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthActions } from '@app/core/store/auth/auth.actions';
import { Store } from '@ngrx/store';
import { AppStore, CategoriesActions } from '@app/core/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import { SwUpdate } from '@angular/service-worker';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
})
export class AppComponent implements OnInit {
  promptEvent: any;
  isAppInstalled: boolean = false;

  private store = inject(Store<AppStore>);
  private swUpdate = inject(SwUpdate);

  ngOnInit() {
    // Check if the app is installed
    window.addEventListener('beforeinstallprompt', event => {
      event.preventDefault();
      this.promptEvent = event;
      this.isAppInstalled = true;
    });

    // Check for updates
    if (this.swUpdate.isEnabled) {
      this.swUpdate.available.subscribe(event => {
        if (confirm('A new version is available. Load it?')) {
          window.location.reload();
        }
      });
    }

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

  addToHomeScreen() {
    if (this.promptEvent) {
      this.promptEvent.prompt();
    } else {
      console.log('App is already installed.');
    }
  }
}
