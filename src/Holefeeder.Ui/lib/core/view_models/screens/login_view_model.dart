import 'dart:async';

import 'package:holefeeder/core/enums/authentication_status_enum.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:holefeeder/core/view_models/base_view_model.dart';
import 'package:provider/provider.dart';

import '../../utils/authentication_client.dart';

class LoginFormState extends BaseFormState {
  LoginFormState({super.state = ViewFormState.initial, super.errorMessage});

  LoginFormState copyWith({ViewFormState? state, String? errorMessage}) {
    return LoginFormState(state: state ?? this.state, errorMessage: errorMessage ?? this.errorMessage);
  }
}

class LoginViewModel extends BaseViewModel {
  final AuthenticationClient _authenticationProvider;
  final _navigationController = StreamController<String>();
  late StreamSubscription<AuthenticationStatus> _statusSubscription;

  LoginFormState _formState = LoginFormState();

  LoginFormState get formState => _formState;

  String get screenTitle => 'Login';

  String get loginTitle => 'Login';

  Stream<String> get navigationStream => _navigationController.stream;

  LoginViewModel({required super.context, AuthenticationClient? authenticationProvider})
    : _authenticationProvider = authenticationProvider ?? Provider.of<AuthenticationClient>(context, listen: false) {
    loadInitialData();
  }

  Future<void> loadInitialData() async {
    _updateState((s) => s.copyWith(state: ViewFormState.loading));

    try {
      _statusSubscription = _authenticationProvider.statusStream.listen((status) {
        if (status == AuthenticationStatus.unauthenticated) {
          _updateState((s) => s.copyWith(state: ViewFormState.initial));
        } else if (status == AuthenticationStatus.authenticated) {
          _updateState((s) => s.copyWith(state: ViewFormState.ready));
        } else if (status == AuthenticationStatus.error) {
          _updateState(
            (s) => s.copyWith(
              state: ViewFormState.error,
              errorMessage: "Failed to login: ${_authenticationProvider.errorMessage}",
            ),
          );
        }
      });
    } catch (e) {
      _updateState((s) => s.copyWith(state: ViewFormState.error, errorMessage: "Failed to load data: $e"));
    }
  }

  @override
  void dispose() {
    _statusSubscription.cancel();
    super.dispose();
  }

  Future<void> login() async {
    _updateState((s) => s.copyWith(state: ViewFormState.loading));
    await _authenticationProvider.login();
    _updateState((s) => s.copyWith(state: ViewFormState.ready));
    _navigationController.add('/');
  }

  void _updateState(LoginFormState Function(LoginFormState) update) {
    _formState = update(_formState);
    notifyListeners();
  }
}
