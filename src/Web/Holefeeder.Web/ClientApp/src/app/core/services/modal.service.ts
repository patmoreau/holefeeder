import { Injectable, Type } from '@angular/core';
import { trace } from '@app/core/logger';
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
  constructor(private ngbModal: NgbModal) {}

  @trace()
  confirm(
    prompt = 'Really?',
    title = 'Confirmation'
  ): Observable<boolean | undefined> {
    return this.custom<ConfirmDialogComponent, boolean>(
      ConfirmDialogComponent,
      { title, prompt }
    );
  }

  @trace()
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

  @trace()
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

  @trace()
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

  @trace()
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
      catchError(error => {
        return of(undefined);
      })
    );
  }
}
