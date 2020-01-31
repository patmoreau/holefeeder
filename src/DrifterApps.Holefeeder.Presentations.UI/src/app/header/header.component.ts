import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DateService } from '@app/singletons/services/date.service';
import { addDays, startOfToday } from 'date-fns';
import { IDateInterval } from '@app/shared/interfaces/date-interval.interface';
import { NgbDate, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserService } from '@app/auth/services/user.service';

@Component({
  selector: 'dfta-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
  period: IDateInterval;

  closeResult: string;

  hoveredDate: NgbDate;

  fromDate: NgbDate;
  toDate: NgbDate;

  isNavbarCollapsed = false;
  isAuthenticated = false;
  givenName: string;

  constructor(
    private modalService: NgbModal,
    private dateService: DateService,
    private userService: UserService,
    private router: Router
  ) {
    this.isNavbarCollapsed = true;

    this.dateService.period.subscribe(period => {
      this.period = period;
    });
  }

  async ngOnInit() {
    this.userService.isAuthenticated$.subscribe(isAuthenticated => {
      this.isAuthenticated = isAuthenticated;
      this.givenName = this.userService.User;
    });
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

  logout() {
    this.userService.logOut();
    this.router.navigateByUrl('/login?redirectUrl=%2Fdashboard');
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
