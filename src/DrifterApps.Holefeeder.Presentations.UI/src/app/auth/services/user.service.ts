import { Injectable } from '@angular/core';
import { OAuthService, OAuthErrorEvent } from 'angular-oauth2-oidc';
import { BehaviorSubject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    public isAuthenticated$ = new BehaviorSubject(false);

    constructor(private oauthService: OAuthService) {
        this.isAuthenticated$.next(this.oauthService.hasValidIdToken());
        this.oauthService.events.subscribe(event => {
            if (event instanceof OAuthErrorEvent) {
                console.error(event);
            } else {
                if (event.type === 'logout') {
                    this.isAuthenticated$.next(this.oauthService.hasValidIdToken());
                } else if (event.type === 'token_received') {
                    this.isAuthenticated$.next(this.oauthService.hasValidIdToken());
                }
            }
        });
    }

    public get User(): string {
        const claims = this.oauthService.getIdentityClaims();
        if (!claims) {
            return null;
        }
        return claims['given_name'];
    }

    public get Token(): string {
        const token: string = this.oauthService.getIdToken();
        if (!token) {
            throw new Error('no token set , authentication required');
        }
        return token;
    }

    public async logIn(): Promise<boolean> {
        return await this.oauthService.tryLoginImplicitFlow();
    }

    public logOut(): void {
        this.oauthService.logOut();
    }
}
