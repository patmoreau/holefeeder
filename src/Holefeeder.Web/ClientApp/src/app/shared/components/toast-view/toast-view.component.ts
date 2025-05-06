import { CommonModule } from '@angular/common';
import {
  Component,
  CUSTOM_ELEMENTS_SCHEMA,
  HostBinding,
  OnInit,
} from '@angular/core';
import { ToastsService } from '@app/core/services';
import { ToastItem, ToastType } from '@app/shared/models';
import { NgbToastModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-toast-view',
  standalone: true,
  imports: [CommonModule, NgbToastModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  templateUrl: './toast-view.component.html',
  styleUrls: ['./toast-view.component.scss']
})
export class ToastViewComponent implements OnInit {
  ToastType = ToastType;

  toasts$!: Observable<ToastItem[]>;

  @HostBinding('class')
  hostClass = 'toast-container position-fixed top-0 end-0 p-3';

  @HostBinding('style')
  hostStyle = 'z-index: 1200';

  constructor(private toastsService: ToastsService) { }

  ngOnInit() {
    this.toasts$ = this.toastsService.toasts$;
  }

  remove(toast: ToastItem) {
    this.toastsService.remove(toast);
  }
}
