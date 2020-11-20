import {Injectable} from '@angular/core';
import {HttpEvent, HttpHandler, HttpRequest} from '@angular/common/http';
import {Observable} from 'rxjs';
import {BroadcastService, MsalInterceptor, MsalService} from "@azure/msal-angular";

@Injectable()
export class AnonymousMsalInterceptor extends MsalInterceptor {
  constructor(auth: MsalService, broadcastService: BroadcastService) {
    super(auth, broadcastService);
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (request.headers.has("Anonymous")) {
      return next.handle(request);
    }
    return super.intercept(request, next);
  }
}
