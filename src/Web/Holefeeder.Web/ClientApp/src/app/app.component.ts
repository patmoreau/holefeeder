import { CommonModule } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { Component, inject, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthActions } from '@app/core/store/auth/auth.actions';
import { Store } from '@ngrx/store';
import { AppStore, CategoriesActions } from '@app/core/store';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import {
  SwUpdate,
  VersionEvent,
  VersionReadyEvent,
} from '@angular/service-worker';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [CommonModule, RouterModule],
})
export class AppComponent implements OnInit {
  isOnline = false;
  modalVersion = false;
  modalPwaEvent: any;
  modalPwaPlatform: string | undefined;

  private store = inject(Store<AppStore>);
  private swUpdate = inject(SwUpdate);
  private platform = inject(Platform);

  ngOnInit() {
    this.updateOnlineStatus();

    window.addEventListener('online', this.updateOnlineStatus.bind(this));
    window.addEventListener('offline', this.updateOnlineStatus.bind(this));

    if (this.swUpdate.isEnabled) {
      this.swUpdate.versionUpdates.pipe(
        filter(
          (evt: VersionEvent): evt is VersionReadyEvent =>
            evt.type === 'VERSION_READY'
        ),
        map((evt: VersionReadyEvent) => {
          console.info(
            `currentVersion=[${evt.currentVersion} | latestVersion=[${evt.latestVersion}]`
          );
          this.modalVersion = true;
        })
      );

      this.loadModalPwa();
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

  private updateOnlineStatus(): void {
    this.isOnline = window.navigator.onLine;
    console.info(`isOnline=[${this.isOnline}]`);
  }

  public updateVersion(): void {
    this.modalVersion = false;
    window.location.reload();
  }

  public closeVersion(): void {
    this.modalVersion = false;
  }

  private loadModalPwa(): void {
    if (this.platform.ANDROID) {
      window.addEventListener('beforeinstallprompt', (event: any) => {
        event.preventDefault();
        this.modalPwaEvent = event;
        this.modalPwaPlatform = 'ANDROID';
      });
    }

    if (this.platform.IOS && this.platform.SAFARI) {
      const isInStandaloneMode =
        'standalone' in window.navigator &&
        (<any>window.navigator)['standalone'];
      if (!isInStandaloneMode) {
        this.modalPwaPlatform = 'IOS';
      }
    }
  }

  public addToHomeScreen(): void {
    this.modalPwaEvent.prompt();
    this.modalPwaPlatform = undefined;
  }

  public closePwa(): void {
    this.modalPwaPlatform = undefined;
  }
}
