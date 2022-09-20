import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ToastItem, ToastType } from '@app/core/models/toast-item.model';
import { ToastsService } from '@app/core/services/toasts.service';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-toast-view',
  standalone: true,
  imports: [CommonModule, NgbToastModule],
  templateUrl: './toast-view.component.html',
  styleUrls: ['./toast-view.component.scss'],
  host: {
    class: 'toast-container position-fixed top-0 end-0 p-3',
    style: 'z-index: 1200',
  },
})
export class ToastViewComponent implements OnInit {
  ToastType = ToastType;

  toasts$!: Observable<ToastItem[]>;

  constructor(private toastsService: ToastsService) {}

  ngOnInit() {
    this.toasts$ = this.toastsService.toasts$;
  }

  remove(toast: ToastItem) {
    this.toastsService.remove(toast);
  }
}
