import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import {
  AuthenticatedResult,
  LoginResponse,
  OidcSecurityService,
  UserDataResult,
} from 'angular-auth-oidc-client';
import { AuthService } from '../auth.service';

describe('AuthService', () => {
  let authService: AuthService;
  let oidcSecurityServiceSpy: jasmine.SpyObj<OidcSecurityService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('OidcSecurityService', [
      'checkAuth',
      'authorize',
      'logoffAndRevokeTokens',
      'getIdToken',
      'isAuthenticated$',
      'userData$',
    ]);

    TestBed.configureTestingModule({
      providers: [AuthService, { provide: OidcSecurityService, useValue: spy }],
    });

    authService = TestBed.inject(AuthService);
    oidcSecurityServiceSpy = TestBed.inject(
      OidcSecurityService
    ) as jasmine.SpyObj<OidcSecurityService>;
  });

  it('should be created', () => {
    expect(authService).toBeTruthy();
  });

  it('should return true for isLoggedIn when isAuthenticated$', (done: DoneFn) => {
    // Arrange
    const isAuthenticatedResult: AuthenticatedResult = {
      allConfigsAuthenticated: [],
      isAuthenticated: true,
    };

    Object.defineProperty(oidcSecurityServiceSpy, 'isAuthenticated$', {
      value: of(isAuthenticatedResult), // Change this to the desired initial state
    });

    // Act & Assert
    authService.isLoggedIn.subscribe(isLoggedIn => {
      expect(isLoggedIn).toBe(true);
      done();
    });
  });

  it('should return id token for token', () => {
    // Arrange
    const idToken = 'mockIdToken';
    oidcSecurityServiceSpy.getIdToken.and.returnValue(of(idToken));

    // Act & Assert
    authService.token.subscribe(token => {
      expect(token).toBe(idToken);
    });
  });

  it('should return user data for userData$', (done: DoneFn) => {
    // Arrange
    const userDataResult: UserDataResult = {
      allUserData: [],
      userData: {
        given_name: 'John',
        family_name: 'Doe',
        name: 'John Doe',
        sub: '123456789',
      },
    };

    Object.defineProperty(oidcSecurityServiceSpy, 'userData$', {
      value: of(userDataResult),
    });

    // Act & Assert
    authService.userData.subscribe(userData => {
      expect(userData.givenName).toBe('John');
      expect(userData.surname).toBe('Doe');
      expect(userData.userPrincipalName).toBe('John Doe');
      expect(userData.id).toBe('123456789');
      done();
    });
  });

  it('should return true for checkAuth when isAuthenticated is true', () => {
    // Arrange
    const loginResponse: LoginResponse = {
      accessToken: '',
      idToken: '',
      userData: undefined,
      isAuthenticated: true,
    };
    oidcSecurityServiceSpy.checkAuth.and.returnValue(of(loginResponse));

    // Act & Assert
    authService.checkAuth().subscribe(result => {
      expect(result).toBe(true);
    });
  });

  it('should do login', () => {
    // Arrange
    oidcSecurityServiceSpy.authorize.and.returnValue();

    // Act & Assert
    authService.doLogin().subscribe(result => {
      expect(oidcSecurityServiceSpy.authorize).toHaveBeenCalled();
      expect(result).toBeUndefined();
    });
  });

  it('should sign out', () => {
    // Arrange
    oidcSecurityServiceSpy.logoffAndRevokeTokens.and.returnValue(of(undefined));

    // Act & Assert
    authService.signOut().subscribe(result => {
      expect(oidcSecurityServiceSpy.logoffAndRevokeTokens).toHaveBeenCalled();
      expect(result).toBeUndefined();
    });
  });
});
