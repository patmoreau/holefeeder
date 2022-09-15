import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StateService } from '@app/core/services/state.service';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { filter, Observable } from 'rxjs';
import { User, UserAdapter } from '../models';
import { MessageService } from './message.service';

const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';

interface UserState {
  user: User | null;
  loggedOn: boolean;
}

const initialState: UserState = {
  user: null,
  loggedOn: false,
};

@Injectable({ providedIn: 'root' })
export class UserService extends StateService<UserState> {
  loggedOn$: Observable<boolean> = this.select(state => state.loggedOn);
  user$: Observable<User | null> = this.select(state => state.user);

  constructor(
    private http: HttpClient,
    public oidcSecurityService: OidcSecurityService,
    private messages: MessageService,
    private adapter: UserAdapter
  ) {
    super(initialState);

    this.oidcSecurityService.isAuthenticated$.subscribe(
      ({ isAuthenticated }) => {
        this.setState({ loggedOn: true });
        console.warn('authenticated: ', isAuthenticated);
      }
    );

    this.oidcSecurityService.userData$
      .pipe(filter(result => result.userData !== null))
      .subscribe(userData => {
        this.setState({ user: this.adapter.adapt(userData.userData) });
      });
  }

  // private loggedOn() {
  //     //  this.setState({ user: user, loggedOn: true })
  //     this.messages.sendMessage({
  //       type: MessageType.general,
  //       action: MessageAction.userLogOn,
  //     });
  //   }
  // }

  // private loggedOff() {
  //   this.setState({ user: null, loggedOn: false });
  //   this.messages.sendMessage({
  //     type: MessageType.general,
  //     action: MessageAction.userLogOff,
  //   });
  // }
}
