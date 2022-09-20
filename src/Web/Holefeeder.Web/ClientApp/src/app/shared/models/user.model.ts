export class User {
  constructor(
    public readonly givenName?: string,
    public readonly surname?: string,
    public readonly userPrincipalName?: string,
    public readonly id?: string
  ) {}
}

