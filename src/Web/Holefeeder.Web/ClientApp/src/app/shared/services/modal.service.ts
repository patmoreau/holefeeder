import {Injectable} from '@angular/core';
import {NgbModal, NgbModalOptions, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {from, Observable} from 'rxjs';

@Injectable()
export class ModalService {

  private ngbModalRef: NgbModalRef | undefined;

  constructor(private ngbModal: NgbModal) {
  }

  open(content: any, options?: NgbModalOptions): Observable<any> {
    this.ngbModalRef = this.ngbModal.open(content, options);

    return from(this.ngbModalRef.result);
  }
}
