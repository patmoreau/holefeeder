import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { AuthenticationService } from './services/authentication.service';

@Injectable()
export class OAuthInterceptor implements HttpInterceptor {
    private secureRoutes = [environment.api_url];

    constructor(
        private authService: AuthenticationService,
    ) {
    }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // Ensure we send the token only to routes which are secured
        if (!this.secureRoutes.find((x) => req.url.startsWith(x))) {
            return next.handle(req);
        }

        const token = this.authService.Token;

        if (!token) {
            return next.handle(req);
        }

        req = req.clone({
            headers: req.headers.set('Authorization', 'Bearer ' + token),
        });

        return next.handle(req);
    }
}
