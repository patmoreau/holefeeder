import { dateToUtc, dateFromUtc } from '../date-parser.helper';
import { User } from '../models/user.model';

export interface IUser {
    id: string;
    firstName: string;
    lastName: string;
    emailAddress: string;
    googleId: string;
    dateJoined: Date;
}

export function userToServer(item: IUser): IUser {
    return Object.assign({} as IUser, item, {
        date: dateToUtc(item.dateJoined)
    });
}

export function userFromServer(item: IUser): IUser {
    return Object.assign(new User(), item, {
        date: dateFromUtc(item.dateJoined)
    });
}
