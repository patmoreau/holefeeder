import 'dart:async';

import 'package:auth0_flutter/auth0_flutter.dart';
import 'package:auth0_flutter/auth0_flutter_web.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/core/constants/strings.dart';
import 'package:rxdart/rxdart.dart';

import '../enums/authentication_status_enum.dart';

abstract class AuthenticationClient {
  static const parameters = {'screen_hint': 'signup'};

  final _statusController = BehaviorSubject<AuthenticationStatus>();

  Credentials? _credentials;
  String? _errorMessage;

  AuthenticationStatus _currentStatus = AuthenticationStatus.unauthenticated;
  AuthenticationStatus get currentStatus => _currentStatus;

  Stream<AuthenticationStatus> get statusStream => _statusController.stream;

  Credentials get credentials => _credentials!;

  UserProfile get userProfile =>
      _credentials?.user ?? UserProfile(sub: 'Unknown');

  String? get errorMessage => _errorMessage;

  @protected
  void setCredentials(Credentials? credentials) {
    _credentials = credentials;
  }

  @protected
  void setStatus(AuthenticationStatus status) {
    _currentStatus = status;
    _statusController.add(status);
  }

  @protected
  void setError(String message) {
    debugPrint('AuthService error: $message');
    _errorMessage = message;
    setStatus(AuthenticationStatus.error);
  }

  @protected
  void clear() {
    _credentials = null;
    _errorMessage = null;
    setStatus(AuthenticationStatus.unauthenticated);
  }

  Future<void> init();

  Future<void> login();

  Future<void> signup();

  Future<void> logout();
}

class MobileAuthenticationClient extends AuthenticationClient {
  final Auth0 _auth0 = Auth0(auth0Domain, auth0ClientId);

  @override
  Future<void> init() async {
    try {
      final isLoggedIn = await _auth0.credentialsManager.hasValidCredentials();
      if (isLoggedIn) {
        final credentials = await _auth0.credentialsManager.credentials();
        setCredentials(credentials);
        setStatus(AuthenticationStatus.authenticated);
      } else {
        setStatus(AuthenticationStatus.unauthenticated);
      }
    } catch (e) {
      setError('Init error: $e');
    }
  }

  @override
  Future<void> login() async {
    setStatus(AuthenticationStatus.loading);
    try {
      final credentials = await _auth0.webAuthentication().login(
        audience: auth0Audience,
        scopes: auth0Scopes,
      );
      setCredentials(credentials);
      setStatus(AuthenticationStatus.authenticated);
    } catch (e) {
      setError('Login error: $e');
    }
  }

  @override
  Future<void> signup() async {
    setStatus(AuthenticationStatus.loading);
    try {
      final credentials = await _auth0.webAuthentication().login(
        audience: auth0Audience,
        scopes: auth0Scopes,
        parameters: AuthenticationClient.parameters,
      );
      setCredentials(credentials);
      setStatus(AuthenticationStatus.authenticated);
    } catch (e) {
      setError('Signup error: $e');
    }
  }

  @override
  Future<void> logout() async {
    setStatus(AuthenticationStatus.loading);
    try {
      await _auth0.webAuthentication().logout();
      clear();
    } catch (e) {
      setError('Logout error: $e');
    }
  }
}

class WebAuthenticationClient extends AuthenticationClient {
  late Auth0Web _auth0;

  @override
  Future<void> init() async {
    _auth0 = Auth0Web(auth0Domain, auth0ClientId);
    setStatus(AuthenticationStatus.unauthenticated);
    try {
      await _auth0
          .onLoad(
            audience: auth0Audience,
            scopes: auth0Scopes,
            cookieDomain: auth0RedirectUriWeb,
            useRefreshTokens: true,
          )
          .then((final credentials) async {
            setCredentials(credentials);
            setStatus(
              credentials == null
                  ? AuthenticationStatus.unauthenticated
                  : AuthenticationStatus.authenticated,
            );
          });
    } catch (e) {
      setError('Init error: $e');
    }
  }

  @override
  Future<void> login() async {
    setStatus(AuthenticationStatus.loading);
    try {
      await _auth0.loginWithRedirect(
        audience: auth0Audience,
        scopes: auth0Scopes,
        redirectUrl: auth0RedirectUriWeb,
      );
    } catch (e) {
      setError('Login error: $e');
    }
  }

  @override
  Future<void> signup() async {
    setStatus(AuthenticationStatus.loading);
    try {
      await _auth0.loginWithRedirect(
        audience: auth0Audience,
        scopes: auth0Scopes,
        redirectUrl: auth0RedirectUriWeb,
        parameters: AuthenticationClient.parameters,
      );
    } catch (e) {
      setError('Signup error: $e');
    }
  }

  @override
  Future<void> logout() async {
    setStatus(AuthenticationStatus.loading);
    try {
      await _auth0.logout(returnToUrl: auth0RedirectUriWeb);
      clear();
    } catch (e) {
      setError('Logout error: $e');
    }
  }
}
