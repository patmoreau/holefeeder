import { Injectable, Type, inject } from '@angular/core';
import {
  ConfirmDialogComponent,
  DeleteDialogComponent,
  InputDialogComponent,
  MessageDialogComponent,
} from '@app/shared/components';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { from, Observable, of, take } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ModalService {
  private ngbModal = inject(NgbModal);


  confirm(
    prompt = 'Really?',
    title = 'Confirmation'
  ): Observable<boolean | undefined> {
    return this.custom<ConfirmDialogComponent, boolean>(
      ConfirmDialogComponent,
      { title, prompt }
    );
  }

  deactivate(
    prompt = 'Deactivate?',
    title = 'Confirmation'
  ): Observable<boolean | undefined> {
    return this.custom<DeleteDialogComponent, boolean>(DeleteDialogComponent, {
      title,
      prompt,
      action: 'Deactivate',
    });
  }

  delete(
    prompt = 'Delete?',
    title = 'Confirmation'
  ): Observable<boolean | undefined> {
    return this.custom<DeleteDialogComponent, boolean>(DeleteDialogComponent, {
      title,
      prompt,
      action: 'Delete',
    });
  }

  input(
    message: string,
    initialValue: string,
    title = 'Input'
  ): Observable<string | undefined> {
    return this.custom<InputDialogComponent, string>(InputDialogComponent, {
      title,
      initialValue,
      message,
    });
  }

  message(message: string, title = 'Message'): Observable<boolean | undefined> {
    return this.custom<MessageDialogComponent, boolean>(
      MessageDialogComponent,
      { title, message }
    );
  }

  custom<T, R>(
    content: Type<T>,
    config?: Partial<T>,
    options?: NgbModalOptions
  ): Observable<R | undefined> {
    // we use a static backdrop by default,
    // but allow the user to set anything in the options
    const modal = this.ngbModal.open(content, {
      backdrop: 'static',
      ...options,
    });

    // copy the config values (if any) into the component
    Object.assign(modal.componentInstance, config);

    return from(modal.result).pipe(
      take(1), // take() manages unsubscription for us
      catchError(() => {
        return of(undefined);
      })
    );
  }
}
