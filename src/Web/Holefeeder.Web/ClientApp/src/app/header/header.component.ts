import { Component, OnDestroy, OnInit } from '@angular/core';
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
import { BroadcastService, MsalService } from '@azure/msal-angular';
import { Account } from 'msal';
import { Subscription } from 'rxjs';
import { SettingsService } from "@app/singletons/services/settings.service";


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

    loggedIn = false;

    subscriptions = new Array<Subscription>();

    constructor(
        private modalService: NgbModal,
        private dateService: DateService,
        private authService: MsalService,
        private settingsService: SettingsService,
        private broadcastService: BroadcastService,
        private router: Router,
    ) {
        this.isNavbarCollapsed = true;
    }

    ngOnInit() {

        this.profile = this.authService.getAccount();
        this.loggedIn = !!this.profile;
        if (this.loggedIn) {
            this.settingsService.loadUserSettings();
        }

        this.dateService.period.subscribe(period => {
            this.period = period;
        });

        this.subscriptions.push(this.broadcastService.subscribe('msal:loginSuccess', (success) => {
            this.profile = this.authService.getAccount();
            this.loggedIn = !!this.profile;
            if (this.loggedIn) {
                this.settingsService.loadUserSettings();
            }
        }));
    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
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
