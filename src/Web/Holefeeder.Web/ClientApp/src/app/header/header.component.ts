import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import { Router } from '@angular/router';
import { DateService } from '@app/singletons/services/date.service';
import { addDays, startOfToday } from 'date-fns';
import { IDateInterval } from '@app/shared/interfaces/date-interval.interface';
import { NgbDate, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
    faTachometerAlt,
    faUniversity,
    faFileInvoiceDollar,
    faChartPie,
    faAngleDown
} from '@fortawesome/free-solid-svg-icons';
import { faCalendarCheck, faCalendarMinus, faCalendarPlus } from '@fortawesome/free-regular-svg-icons';
import {Subject} from 'rxjs';
import { SettingsService } from "@app/singletons/services/settings.service";
import {MSAL_GUARD_CONFIG, MsalBroadcastService, MsalGuardConfiguration, MsalService} from "@azure/msal-angular";
import {
  AuthenticationResult, AuthError,
  EventMessage,
  EventType,
  InteractionType,
  PopupRequest,
  RedirectRequest
} from "@azure/msal-browser";
import {b2cPolicies} from "@app/config/config.service";
import {filter, takeUntil} from "rxjs/operators";

interface IdTokenClaims extends AuthenticationResult {
  idTokenClaims: {
    acr?: string
  }
}

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
    profile: Account;

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
    private readonly _destroying$ = new Subject<void>();

    constructor(
        private modalService: NgbModal,
        private dateService: DateService,
        private settingsService: SettingsService,
        private router: Router,
        @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
        private authService: MsalService,
        private msalBroadcastService: MsalBroadcastService
    ) {
        this.isNavbarCollapsed = true;
    }

    ngOnInit() {
      this.isIframe = window !== window.parent && !window.opener;

      this.checkAccount();

      this.msalBroadcastService.msalSubject$
        .pipe(
          filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS || msg.eventType === EventType.ACQUIRE_TOKEN_SUCCESS),
          takeUntil(this._destroying$)
        )
        .subscribe((result: EventMessage) => {

          let payload: IdTokenClaims = <AuthenticationResult>result.payload;

          // We need to reject id tokens that were not issued with the default sign-in policy.
          // "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr")
          // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview

          if (payload.idTokenClaims?.acr === b2cPolicies.names.forgotPassword) {
            window.alert('Password has been reset successfully. \nPlease sign-in with your new password.');
            return this.authService.logout();
          } else if (payload.idTokenClaims['acr'] === b2cPolicies.names.editProfile) {
            window.alert('Profile has been updated successfully. \nPlease sign-in again.');
            return this.authService.logout();
          }

          this.checkAccount();
          return result;
        });

      this.msalBroadcastService.msalSubject$
        .pipe(
          filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_FAILURE || msg.eventType === EventType.ACQUIRE_TOKEN_FAILURE),
          takeUntil(this._destroying$)
        )
        .subscribe((result: EventMessage) => {
          if (result.error instanceof AuthError) {
            // Check for forgot password error
            // Learn more about AAD error codes at https://docs.microsoft.com/azure/active-directory/develop/reference-aadsts-error-codes
            if (result.error.message.includes('AADB2C90118')) {

              // login request with reset authority
              let resetPasswordFlowRequest = {
                scopes: ["openid"],
                authority: b2cPolicies.authorities.forgotPassword.authority,
              }

              this.login(resetPasswordFlowRequest);
            }
          }
        });

        if (this.loggedIn) {
            this.settingsService.loadUserSettings();
        }

        this.dateService.period.subscribe(period => {
            this.period = period;
        });
    }

    ngOnDestroy(): void {
      this._destroying$.next(undefined);
      this._destroying$.complete();
    }

    checkAccount() {
      this.loggedIn = this.authService.instance.getAllAccounts().length > 0;
    }

    login(userFlowRequest?: RedirectRequest | PopupRequest) {
      this.msalGuardConfig
      if (this.msalGuardConfig.interactionType === InteractionType.Popup) {
        if (this.msalGuardConfig.authRequest) {
          this.authService.loginPopup({...this.msalGuardConfig.authRequest, ...userFlowRequest} as PopupRequest)
            .subscribe((response: AuthenticationResult) => {
              this.authService.instance.setActiveAccount(response.account);
              this.checkAccount();
            });
        } else {
          this.authService.loginPopup(userFlowRequest)
            .subscribe((response: AuthenticationResult) => {
              this.authService.instance.setActiveAccount(response.account);
              this.checkAccount();
            });
        }
      } else {
        if (this.msalGuardConfig.authRequest){
          this.authService.loginRedirect({...this.msalGuardConfig.authRequest, ...userFlowRequest} as RedirectRequest);
        } else {
          this.authService.loginRedirect(userFlowRequest);
        }
      }
    }

    logout() {
      this.authService.logout();
    }

    editProfile() {
      let editProfileFlowRequest = {
        scopes: ["openid"],
        authority: b2cPolicies.authorities.editProfile.authority,
      }

      this.login(editProfileFlowRequest);
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
                _ => {
                }
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
