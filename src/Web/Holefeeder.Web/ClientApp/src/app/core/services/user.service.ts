import {Injectable} from "@angular/core";
import {MessageService, User, UserAdapter} from "@app/core";
import {StateService} from "@app/core/services/state.service";
import {filter, map, Observable, of} from "rxjs";
import {AuthenticationResult, EventMessage, EventType, InteractionStatus} from "@azure/msal-browser";
import {MsalBroadcastService, MsalService} from "@azure/msal-angular";
import {HttpClient} from "@angular/common/http";
import {MessageAction, MessageType} from "@app/shared";

const GRAPH_ENDPOINT = 'https://graph.microsoft.com/v1.0/me';

interface UserState {
  user: User | null;
  loggedOn: boolean;
}

const initialState: UserState = {
  user: null,
  loggedOn: false
};

@Injectable({providedIn: 'root'})
export class UserService extends StateService<UserState> {

  loggedOn$: Observable<boolean> = this.select((state) => state.loggedOn);
  user$: Observable<User | null> = this.select((state) => state.user);

  constructor(
    private http: HttpClient,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
    private messages: MessageService,
    private adapter: UserAdapter
  ) {
    super(initialState);

    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.LOGIN_SUCCESS),
      )
      .subscribe((result: EventMessage) => {
        const payload = result.payload as AuthenticationResult;
        this.authService.instance.setActiveAccount(payload.account);
      });

    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.LOGOUT_SUCCESS),
      )
      .subscribe((result: EventMessage) => {
        this.loggedOff()
      });

    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None)
      )
      .subscribe(() => {
        this.loggedOn()
      });
  }

  private loggedOn() {
    if (this.authService.instance.getAllAccounts().length > 0) {
      this.authService.instance.setActiveAccount(this.authService.instance.getAllAccounts()[0])
      this.getUserProfile().subscribe(user => this.setState({user: user, loggedOn: true}))
      this.messages.sendMessage(MessageType.general, MessageAction.userLogOn);
    }
  }

  private loggedOff() {
    this.setState({user: null, loggedOn: false});
    this.messages.sendMessage(MessageType.general, MessageAction.userLogOff);
  }

  private getUserProfile(): Observable<User> {
    return of(this.authService.instance.getActiveAccount())
      .pipe(
        filter(accountInfo => accountInfo !== null),
        map(accountInfo => this.adapter.adapt(accountInfo!.idTokenClaims))
      );
    // return this.http.get(GRAPH_ENDPOINT)
    //   .pipe(map(this.adapter.adapt));
  }
}
