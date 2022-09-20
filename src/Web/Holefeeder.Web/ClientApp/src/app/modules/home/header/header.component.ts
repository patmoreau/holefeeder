import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { logger } from '@app/core';
import { DateInterval } from '@app/core/models/date-interval.model';
import { Settings } from '@app/core/models/settings.model';
import { User } from '@app/core/models/user.model';
import {
  SettingsService,
  SubscriberService,
  UserService,
} from '@app/core/services';
import { NgbDate, NgbModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { addDays, startOfToday } from 'date-fns';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
  settings$: Observable<Settings> | undefined;
  period$: Observable<DateInterval> | undefined;

  logged$: Observable<boolean>;
  user$: Observable<User | null>;

  hoveredDate: NgbDate | null = null;
  fromDate: NgbDate | null = null;
  toDate: NgbDate | null = null;

  isNavbarCollapsed = false;

  loggedIn = false;

  constructor(
    private http: HttpClient,
    public oidcSecurityService: OidcSecurityService,
    private modalService: NgbModal,
    private settingsService: SettingsService,
    private router: Router,
    private userService: UserService,
    private subService: SubscriberService
  ) {
    this.isNavbarCollapsed = true;

    this.user$ = this.userService.user$;
    this.logged$ = this.userService.loggedOn$;
    this.settings$ = this.settingsService.settings$;
    this.period$ = this.settingsService.period$;
  }

  ngOnInit() {}

  open(content: any, period: DateInterval) {
    this.setCalendar(period);
    this.modalService
      .open(content, { ariaLabelledBy: 'modal-basic-title' })
      .result.then(
        _ => {
          this.settingsService.setPeriod(
            new DateInterval(
              this.getDate(this.fromDate!),
              this.getDate(this.toDate!)
            )
          );
        },
        _ => {}
      );
  }

  nextPeriod() {
    if (this.toDate === undefined) {
      logger.error('No date defined');
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
      logger.error('No date defined');
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
    this.oidcSecurityService.authorize();
  }

  refreshSession() {
    this.subService.add(
      this.oidcSecurityService
        .forceRefreshSession()
        .subscribe(result => console.log(result))
    );
  }

  logout() {
    this.oidcSecurityService.logoff();
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
}
