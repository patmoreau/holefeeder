import { IUser } from '../interfaces/user.interface';

export class User implements IUser {
    id: string;
    firstName: string;
    lastName: string;
    emailAddress: string;
    googleId: string;
    dateJoined: Date;
}
