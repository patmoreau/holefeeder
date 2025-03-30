import 'dart:async';

import '../../enums/authentication_status_enum.dart';
import '../base_view_model.dart';

class LoginViewModel extends BaseViewModel {
  final action = 'Login';

  late StreamSubscription<AuthenticationStatus> _statusSubscription;

  Stream<AuthenticationStatus> get authenticationStatusStream =>
      authenticationProvider.statusStream;

  String? get errorMessage => authenticationProvider.errorMessage;

  LoginViewModel({required super.context}) {
    _statusSubscription = authenticationProvider.statusStream.listen((status) {
      notifyListeners();
    });
  }

  @override
  void dispose() {
    _statusSubscription.cancel();
    super.dispose();
  }

  Future<void> login() async {
    notifyListeners();
    await authenticationProvider.login();
    notifyListeners();
  }
}
