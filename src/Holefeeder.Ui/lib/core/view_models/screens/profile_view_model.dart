import 'dart:async';

import '../../enums/authentication_status_enum.dart';
import '../base_view_model.dart';

class ProfilViewModel extends BaseViewModel {
  final action = 'Login';
  String _pictureUrl = '';
  String _name = '';
  String? _errorMessage;

  late StreamSubscription<AuthenticationStatus> _statusSubscription;

  Stream<AuthenticationStatus> get authenticationStatusStream =>
      authenticationProvider.statusStream;

  String get name => _name;
  String get pictureUrl => _pictureUrl;
  String? get errorMessage => _errorMessage;

  ProfilViewModel({required super.context}) {
    _statusSubscription = authenticationProvider.statusStream.listen((status) {
      if (status == AuthenticationStatus.unauthenticated) {
        _name = '';
        _pictureUrl = '';
      } else if (status == AuthenticationStatus.authenticated) {
        final user = authenticationProvider.credentials.user;
        _name = user.name?.toString() ?? '';
        _pictureUrl = user.pictureUrl?.toString() ?? '';
      }
      _errorMessage = authenticationProvider.errorMessage;
      notifyListeners();
    });
  }

  @override
  void dispose() {
    _statusSubscription.cancel();
    super.dispose();
  }

  Future<void> logout() async {
    notifyListeners();
    await authenticationProvider.logout();
    notifyListeners();
  }

  void fallbackToDefaultPicture() {
    _pictureUrl = '';
    notifyListeners();
  }
}
