import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export class User {
  constructor(
    public readonly givenName?: string,
    public readonly surname?: string,
    public readonly userPrincipalName?: string,
    public readonly id?: string) {
  }
}

@Injectable({providedIn: 'root'})
export class UserAdapter implements Adapter<User> {
  adapt(item: any): User {
    return new User(item.given_name, item.family_name, item.name, item.sub);
  }
}
