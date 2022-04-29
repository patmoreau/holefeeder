import {Injectable, Type} from '@angular/core';
import {NgbModal, NgbModalOptions} from '@ng-bootstrap/ng-bootstrap';
import {Observable, from, of, take} from 'rxjs';
import {catchError} from 'rxjs/operators';
import {ConfirmDialogComponent} from "@app/shared/components/modals/confirm-dialog/confirm-dialog.component";
import {InputDialogComponent} from "@app/shared/components/modals/input-dialog/input-dialog.component";
import {MessageDialogComponent} from "@app/shared/components/modals/message-dialog/message-dialog.component";
import {DeactivateDialogComponent} from "@app/shared/components/modals/deactivate-dialog/deactivate-dialog.component";

@Injectable({providedIn: 'root'})
export class ModalService {

  constructor(private ngbModal: NgbModal) {
  }

  confirm(prompt = 'Really?', title = 'Confirmation'): Observable<boolean | undefined> {
    return this.custom<ConfirmDialogComponent, boolean>(ConfirmDialogComponent, {title, prompt});
  }

  deactivate(prompt = 'Deactivate?', title = 'Confirmation'): Observable<boolean | undefined> {
    return this.custom<DeactivateDialogComponent, boolean>(DeactivateDialogComponent, {title, prompt});
  }

  input(message: string, initialValue: string, title = 'Input'): Observable<string | undefined> {
    return this.custom<InputDialogComponent, string>(InputDialogComponent, {title, initialValue, message});
  }

  message(message: string, title = 'Message'): Observable<boolean | undefined> {
    return this.custom<MessageDialogComponent, boolean>(MessageDialogComponent, {title, message});
  }

  custom<T, R>(
    content: Type<T>,
    config?: Partial<T>,
    options?: NgbModalOptions
  ): Observable<R | undefined> {
    // we use a static backdrop by default,
    // but allow the user to set anything in the options
    const modal = this.ngbModal.open(content, {backdrop: 'static', ...options}
    );

    // copy the config values (if any) into the component
    Object.assign(modal.componentInstance, config);

    return from(modal.result).pipe(
      take(1), // take() manages unsubscription for us
      catchError(error => {
        console.warn(error);
        return of(undefined);
      })
    );
  }
}
