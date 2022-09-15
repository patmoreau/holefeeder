import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { User } from "@app/core/models/user.model";

@Injectable({ providedIn: "root" })
export class UserAdapter implements Adapter<User> {
  adapt(item: any): User {
    return new User(item.given_name, item.family_name, item.name, item.sub);
  }
}
