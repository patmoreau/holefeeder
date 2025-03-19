import 'package:auth0_flutter/auth0_flutter.dart';
import 'package:auth0_flutter/auth0_flutter_web.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/helpers/constants.dart';

enum AuthStatus { unauthenticated, authenticated, loading }

abstract class AuthService with ChangeNotifier {
  static const parameters = {'screen_hint': 'signup'};

  Credentials? _credentials;
  AuthStatus _authStatus = AuthStatus.unauthenticated;
  String? _errorMessage;

  Credentials get credentials => _credentials!;

  UserProfile get userProfile => _credentials?.user ?? UserProfile(sub: 'Unknown');

  AuthStatus get authStatus => _authStatus;

  String? get errorMessage => _errorMessage;

  @protected
  void setCredentials(Credentials? credentials) {
    _credentials = credentials;
  }

  @protected
  void setStatus(AuthStatus status) {
    _authStatus = status;
    notifyListeners();
  }

  @protected
  void setError(String message) {
    debugPrint('AuthService error: $message');
    _errorMessage = message;
    _authStatus = AuthStatus.unauthenticated;
    notifyListeners();
  }

  @protected
  void clear() {
    _credentials = null;
    _errorMessage = null;
    _authStatus = AuthStatus.unauthenticated;
    notifyListeners();
  }

  void init();

  Future<void> login();

  Future<void> signup();

  Future<void> logout();
}

class MobileAuthService extends AuthService {
  final Auth0 _auth0 = Auth0(auth0Domain!, auth0ClientId!);

  @override
  void init() async {
    try {
      final isLoggedIn = await _auth0.credentialsManager.hasValidCredentials();
      if (isLoggedIn) {
        final credentials = await _auth0.credentialsManager.credentials();
        setCredentials(credentials);
        setStatus(AuthStatus.authenticated);
      }
    } catch (e) {
      setError('Init error: $e');
    }
  }

  @override
  Future<void> login() async {
    setStatus(AuthStatus.loading);
    try {
      final credentials = await _auth0.webAuthentication().login(audience: auth0Audience, scopes: auth0Scopes);
      setCredentials(credentials);
      setStatus(AuthStatus.authenticated);
    } catch (e) {
      setError('Login error: $e');
    }
  }

  @override
  Future<void> signup() async {
    setStatus(AuthStatus.loading);
    try {
      final credentials = await _auth0.webAuthentication().login(
        audience: auth0Audience,
        scopes: auth0Scopes,
        parameters: AuthService.parameters,
      );
      setCredentials(credentials);
      setStatus(AuthStatus.authenticated);
    } catch (e) {
      setError('Signup error: $e');
    }
  }

  @override
  Future<void> logout() async {
    setStatus(AuthStatus.loading);
    try {
      await _auth0.webAuthentication().logout();
      clear();
    } catch (e) {
      setError('Logout error: $e');
    }
  }
}

class WebAuthService extends AuthService {
  late Auth0Web _auth0;

  @override
  void init() {
    _auth0 = Auth0Web(auth0Domain!, auth0ClientId!);
    try {
      _auth0.onLoad().then((final credentials) {
        debugPrint('Got myself some credentials: ${credentials?.user.name}');
        setCredentials(credentials);
        setStatus(credentials == null ? AuthStatus.unauthenticated : AuthStatus.authenticated);
      });
    } catch (e) {
      setError('Init error: $e');
    }
  }

  @override
  Future<void> login() async {
    setStatus(AuthStatus.loading);
    try {
      await _auth0.loginWithRedirect(audience: auth0Audience, scopes: auth0Scopes, redirectUrl: auth0RedirectUriWeb);
    } catch (e) {
      setError('Login error: $e');
    }
  }

  @override
  Future<void> signup() async {
    setStatus(AuthStatus.loading);
    try {
      await _auth0.loginWithRedirect(
        audience: auth0Audience,
        scopes: auth0Scopes,
        redirectUrl: auth0RedirectUriWeb,
        parameters: AuthService.parameters,
      );
    } catch (e) {
      setError('Signup error: $e');
    }
  }

  @override
  Future<void> logout() async {
    setStatus(AuthStatus.loading);
    try {
      await _auth0.logout(returnToUrl: auth0RedirectUriWeb);
      clear();
    } catch (e) {
      setError('Logout error: $e');
    }
  }
}
