import { Injectable } from '@angular/core';
import { User } from '@app/core/models/user.model';
import { Adapter } from '@app/shared/models';

@Injectable({ providedIn: 'root' })
export class UserAdapter implements Adapter<User> {
  adapt(item: any): User {
    return new User(item.given_name, item.family_name, item.name, item.sub);
  }
}
