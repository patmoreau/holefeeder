
import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { SettingsService, SubscriberService } from '@app/core/services';
import {
  DateInterval,
  DateIntervalTypeNames,
  Settings,
  User,
} from '@app/shared/models';
import { NgbDate, NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { addDays, startOfToday } from 'date-fns';
import { Observable, of } from 'rxjs';
import { Store } from '@ngrx/store';
import { AppStore } from '@app/core/store';
import { AuthActions } from '@app/core/store/auth/auth.actions';
import { AuthFeature } from '@app/core/store/auth/auth.feature';
import { LoggerService } from '@app/core/logger';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    RouterModule,
    NgbModule,
    ReactiveFormsModule
],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.scss']
})
export class UnauthorizedComponent implements OnInit {
  oidcSecurityService = inject(OidcSecurityService);
  private modalService = inject(NgbModal);
  private settingsService = inject(SettingsService);
  private router = inject(Router);
  private subService = inject(SubscriberService);
  private store = inject<Store<AppStore>>(Store);
  private logger = inject(LoggerService);

  settings$: Observable<Settings> | undefined;
  period$: Observable<DateInterval> | undefined;

  logged$: Observable<boolean> = of(false);
  user$: Observable<User | null> = of(null);

  hoveredDate: NgbDate | null = null;
  fromDate: NgbDate | null = null;
  toDate: NgbDate | null = null;

  isNavbarCollapsed = false;

  loggedIn = false;

  ngOnInit() {
    this.isNavbarCollapsed = true;

    this.user$ = this.store.select(AuthFeature.selectProfile);
    this.logged$ = this.store.select(AuthFeature.selectIsAuthenticated);
    this.settings$ = this.settingsService.settings$;
    this.period$ = this.settingsService.period$;
  }

  open(content: unknown, period: DateInterval) {
    this.setCalendar(period);
    this.modalService
      .open(content, { ariaLabelledBy: 'modal-basic-title' })
      .result.then(() => {
        this.settingsService.setPeriod(
          new DateInterval(
            this.getDate(this.fromDate!),
            this.getDate(this.toDate!)
          )
        );
      });
  }

  nextPeriod() {
    if (this.toDate === undefined) {
      this.logger.error('No date defined');
      return;
    }
    const date = this.getDate(this.toDate!);
    const period = this.settingsService.getPeriod(addDays(date, 2));

    this.setCalendar(period);
  }

  currentPeriod() {
    const period = this.settingsService.getPeriod(startOfToday());

    this.setCalendar(period);
  }

  previousPeriod() {
    if (this.fromDate === undefined) {
      this.logger.error('No date defined');
      return;
    }
    const date = this.getDate(this.fromDate!);
    const period = this.settingsService.getPeriod(addDays(date, -1));

    this.setCalendar(period);
  }

  click(routeLink: string) {
    this.isNavbarCollapsed = !this.isNavbarCollapsed;
    this.router.navigate([routeLink]);
  }

  onDateSelection(date: NgbDate) {
    if (!this.fromDate && !this.toDate) {
      this.fromDate = date;
    } else if (this.fromDate && !this.toDate && date.after(this.fromDate)) {
      this.toDate = date;
    } else {
      this.toDate = null;
      this.fromDate = date;
    }
  }

  isHovered(date: NgbDate) {
    return (
      this.fromDate &&
      !this.toDate &&
      this.hoveredDate &&
      date.after(this.fromDate) &&
      date.before(this.hoveredDate)
    );
  }

  isInside(date: NgbDate) {
    return date.after(this.fromDate) && date.before(this.toDate);
  }

  isRange(date: NgbDate) {
    return (
      date.equals(this.fromDate) ||
      date.equals(this.toDate) ||
      this.isInside(date) ||
      this.isHovered(date)
    );
  }

  login() {
    this.store.dispatch(AuthActions.login());
  }

  refreshSession() {
    this.subService.add(
      this.oidcSecurityService
        .forceRefreshSession()
        .subscribe(result => console.log(result))
    );
  }

  logout() {
    this.store.dispatch(AuthActions.logout());
  }

  private setCalendar(period: DateInterval) {
    if (period === null) {
      return;
    }
    this.fromDate = new NgbDate(
      period.start.getFullYear(),
      period.start.getMonth() + 1,
      period.start.getDate()
    );
    this.toDate = new NgbDate(
      period.end.getFullYear(),
      period.end.getMonth() + 1,
      period.end.getDate()
    );
  }

  private getDate(ngbDate: NgbDate): Date {
    const jsDate = new Date(ngbDate.year, ngbDate.month - 1, ngbDate.day);
    jsDate.setFullYear(ngbDate.year);
    return jsDate;
  }

  protected readonly intervalTypesNames = DateIntervalTypeNames;
}
