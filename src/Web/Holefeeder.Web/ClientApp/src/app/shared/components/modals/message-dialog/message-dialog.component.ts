import {ChangeDetectionStrategy, Component} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-message-dialog',
  template: `
    <div>
      <div class="modal-header">
        <h4 class="modal-title">{{title}}</h4>
      </div>
      <div class="modal-body">
        <p>{{message}}</p>
      </div>
      <div class="modal-footer">
        <button type="button"
                class="btn btn-outline-dark"
                (click)="activeModal.close(true)">OK
        </button>
      </div>
    </div>`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MessageDialogComponent {
  title!: string;
  message!: string;

  constructor(public activeModal: NgbActiveModal) {
  }
}
