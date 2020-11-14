import { Component, OnDestroy, OnInit } from '@angular/core';
import { BroadcastService, MsalService } from '@azure/msal-angular';
import { CryptoUtils, Logger } from 'msal';
import { Subscription } from 'rxjs';
import { b2cPolicies } from './app-config';

@Component({
    selector: 'dfta-root',
    templateUrl: './app.component.html',
})
export class AppComponent implements OnInit, OnDestroy {
    loggedIn = false;

    subscriptions = new Array<Subscription>();

    constructor(
        private authService: MsalService,
        private broadcastService: BroadcastService
    ) {
    }

    ngOnInit() {
        let loginSuccessSubscription: Subscription;
        let loginFailureSubscription: Subscription;

        // event listeners for authentication status
        loginSuccessSubscription = this.broadcastService.subscribe('msal:loginSuccess', (success) => {

            // We need to reject id tokens that were not issued with the default sign-in policy.
            // "acr" claim in the token tells us what policy is used (NOTE: for new policies (v2.0), use "tfp" instead of "acr")
            // To learn more about b2c tokens, visit https://docs.microsoft.com/en-us/azure/active-directory-b2c/tokens-overview
            if (success.idToken.claims.acr === b2cPolicies.names.resetPassword) {
                window.alert('Password has been reset successfully. \nPlease sign-in with your new password');
                return this.authService.logout();
            }
        });

        loginFailureSubscription = this.broadcastService.subscribe('msal:loginFailure', (error) => {
            console.log('login failed');
            console.log(error);

            if (error.errorMessage) {
                // Check for forgot password error
                // Learn more about AAD error codes at https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
                if (error.errorMessage.indexOf('AADB2C90118') > -1) {
                    this.authService.loginRedirect(b2cPolicies.authorities.resetPassword);
                }
            }
        });

        // redirect callback for redirect flow (IE)
        this.authService.handleRedirectCallback((authError, response) => {
            if (authError) {
                console.error('Redirect Error: ', authError.errorMessage);
                return;
            }
        });

        this.authService.setLogger(new Logger((logLevel, message, piiEnabled) => {
            console.log('MSAL Logging: ', message);
        }, {
            correlationId: CryptoUtils.createNewGuid(),
            piiLoggingEnabled: false
        }));

        this.subscriptions.push(loginSuccessSubscription);
        this.subscriptions.push(loginFailureSubscription);

    }

    ngOnDestroy(): void {
        this.subscriptions.forEach((subscription) => subscription.unsubscribe());
    }
}