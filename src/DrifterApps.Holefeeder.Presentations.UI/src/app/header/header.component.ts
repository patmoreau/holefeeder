import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DateService } from '@app/singletons/services/date.service';
import { addDays, startOfToday } from 'date-fns';
import { IDateInterval } from '@app/shared/interfaces/date-interval.interface';
import { NgbDate, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { faTachometerAlt, faUniversity, faFileInvoiceDollar, faChartPie, faAngleDown } from '@fortawesome/free-solid-svg-icons';
import { faCalendarCheck, faCalendarMinus, faCalendarPlus } from '@fortawesome/free-regular-svg-icons';
import { SettingsService } from '@app/singletons/services/settings.service';
import { BroadcastService, MsalService } from '@azure/msal-angular';
import { CryptoUtils, Logger } from 'msal';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { b2cPolicies, isIE } from '@app/app-config';

const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';

@Component({
  selector: 'dfta-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  period: IDateInterval;

  closeResult: string;

  hoveredDate: NgbDate;

  fromDate: NgbDate;
  toDate: NgbDate;

  isNavbarCollapsed = false;
  profile;

  faTachometerAlt = faTachometerAlt;
  faUniversity = faUniversity;
  faFileInvoiceDollar = faFileInvoiceDollar;
  faChartPie = faChartPie;
  faAngleDown = faAngleDown;
  faCalendarMinus = faCalendarMinus;
  faCalendarCheck = faCalendarCheck;
  faCalendarPlus = faCalendarPlus;
  isIframe = false;
  loggedIn = false;

  subscriptions: Subscription[] = [];

  constructor(
    private modalService: NgbModal,
    private dateService: DateService,
    private settingsService: SettingsService,
    private broadcastService: BroadcastService,
    private authService: MsalService,
    private router: Router,
    private http: HttpClient
  ) {
    this.isNavbarCollapsed = true;
  }

  async ngOnInit() {
    let loginSuccessSubscription: Subscription;
    let loginFailureSubscription: Subscription;

    this.isIframe = window !== window.parent && !window.opener;
    this.checkAccount();

    this.dateService.period.subscribe(period => {
      this.period = period;
    });

    // event listeners for authentication status
    loginSuccessSubscription = this.broadcastService.subscribe('msal:loginSuccess', (success) => {

      // We need to reject id tokens that were not issued with the default sign-in policy.
      // "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr")
      // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
      if (success.idToken.claims.acr === b2cPolicies.names.resetPassword) {
        window.alert('Password has been reset successfully. \nPlease sign-in with your new password');
        return this.authService.logout();
      }

      this.checkAccount();
    });

    loginFailureSubscription = this.broadcastService.subscribe('msal:loginFailure', (error) => {
      console.log('login failed');
      console.log(error);

      if (error.errorMessage) {
        // Check for forgot password error
        // Learn more about AAD error codes at https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
        if (error.errorMessage.indexOf('AADB2C90118') > -1) {
          if (isIE) {
            this.authService.loginRedirect(b2cPolicies.authorities.resetPassword);
          } else {
            this.authService.loginPopup(b2cPolicies.authorities.resetPassword);
          }
        }
      }
    });

    // redirect callback for redirect flow (IE)
    this.authService.handleRedirectCallback((authError, response) => {
      if (authError) {
        console.error('Redirect Error: ', authError.errorMessage);
        return;
      }
    });

    this.authService.setLogger(new Logger((logLevel, message, piiEnabled) => {
      console.log('MSAL Logging: ', message);
    }, {
      correlationId: CryptoUtils.createNewGuid(),
      piiLoggingEnabled: false
    }));

    this.subscriptions.push(loginSuccessSubscription);
    this.subscriptions.push(loginFailureSubscription);
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach((subscription) => subscription.unsubscribe());
  }

  checkAccount() {
    this.profile = this.authService.getAccount();
    this.loggedIn = !!this.profile;
    if (this.loggedIn) {
      this.settingsService.loadUserSettings();
    }
  }

  login() {
    if (isIE) {
      this.authService.loginRedirect();
    } else {
      this.authService.loginPopup();
    }
  }

  logout() {
    this.authService.logout();
  }

  open(content) {
    this.setCalendar(this.period);
    this.modalService
      .open(content, { ariaLabelledBy: 'modal-basic-title' })
      .result.then(
        _ => {
          this.dateService.setPeriod({
            start: this.getDate(this.fromDate),
            end: this.getDate(this.toDate)
          } as IDateInterval);
        },
        _ => { }
      );
  }

  nextPeriod() {
    const date = this.getDate(this.toDate);
    const period = this.dateService.getPeriod(addDays(date, 2));

    this.setCalendar(period);
  }

  currentPeriod() {
    const period = this.dateService.getPeriod(startOfToday());

    this.setCalendar(period);
  }

  previousPeriod() {
    const date = this.getDate(this.fromDate);
    const period = this.dateService.getPeriod(addDays(date, -1));

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

  private setCalendar(period: IDateInterval) {
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
