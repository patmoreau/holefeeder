import {Injectable} from '@angular/core';
import {NgbModal, NgbModalOptions, NgbModalRef} from '@ng-bootstrap/ng-bootstrap';
import {Observable, of, Subject} from 'rxjs';

@Injectable()
export class ModalService {

  private modalsMap = new Map<string, NgbModalRef>();
  private ngbModalRef!: NgbModalRef;

  public onClosed$ = new Subject<any>();
  public onDismissed$ = new Subject<any>();

  constructor(private ngbModal: NgbModal) {
  }

  open(content: any, options?: NgbModalOptions): Observable<any> {
    this.ngbModalRef = this.ngbModal.open(content, options);
    this.ngbModalRef.closed.subscribe((value: any) => this.onClosed$.next(value));
    this.ngbModalRef.dismissed.subscribe((value: any) => this.onDismissed$.next(value));

    return of(this.ngbModalRef.result);
  }

  openNew(id: string, content: any, options?: NgbModalOptions): void {
    if (this.modalsMap.has(id)) {
      return;
    }

    const ngbModalRef = this.ngbModal.open(content, options);
    this.modalsMap.set(id, ngbModalRef);
  }

  close(id: string): boolean {
    if (!this.modalsMap.has(id)) {
      return false;
    }
    const ngbModalRef = this.modalsMap.get(id);
    ngbModalRef!.close()
    return true;
  }

  dismiss(id: string): boolean {
    if (!this.modalsMap.has(id)) {
      return false;
    }
    const ngbModalRef = this.modalsMap.get(id);
    ngbModalRef!.dismiss()
    return true;
  }
}
