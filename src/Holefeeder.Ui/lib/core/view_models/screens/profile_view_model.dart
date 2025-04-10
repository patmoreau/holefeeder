import 'dart:async';

import 'package:holefeeder/core/utils/authentication_client.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:provider/provider.dart';

import '../../enums/authentication_status_enum.dart';
import '../base_view_model.dart';

class ProfileFormState extends BaseFormState {
  final String name;
  final String pictureUrl;

  ProfileFormState({
    this.name = '',
    this.pictureUrl = '',
    super.state = ViewFormState.initial,
    super.errorMessage,
  });

  ProfileFormState copyWith({
    String? name,
    String? pictureUrl,
    ViewFormState? state,
    String? errorMessage,
  }) {
    return ProfileFormState(
      name: name ?? this.name,
      pictureUrl: pictureUrl ?? this.pictureUrl,
      state: state ?? this.state,
      errorMessage: errorMessage ?? this.errorMessage,
    );
  }
}

class ProfileViewModel extends BaseViewModel {
  final AuthenticationClient _authenticationProvider;
  final _navigationController = StreamController<String>();
  Stream<String> get navigationStream => _navigationController.stream;

  ProfileFormState _formState = ProfileFormState();
  ProfileFormState get formState => _formState;

  late StreamSubscription<AuthenticationStatus> _statusSubscription;

  ProfileViewModel({required super.context, AuthenticationClient? authenticationProvider})
      : _authenticationProvider = authenticationProvider ?? Provider.of<AuthenticationClient>(context, listen: false)  {
    loadInitialData();
  }

  String get screenTitle => 'Profile';
  String get logoutTitle => 'Logout';
  String get fallbackPictureUrl => 'images/default_profile.png';

  Future<void> loadInitialData() async {
    _updateState((s) => s.copyWith(state: ViewFormState.loading));

    try {
      _statusSubscription = _authenticationProvider.statusStream.listen((
        status,
      ) {
        if (status == AuthenticationStatus.unauthenticated) {
          _updateState(
            (s) => s.copyWith(
              state: ViewFormState.initial,
              name: '',
              pictureUrl: '',
            ),
          );
        } else if (status == AuthenticationStatus.authenticated) {
          final user = _authenticationProvider.credentials.user;
          _updateState(
            (s) => s.copyWith(
              state: ViewFormState.ready,
              name: user.name?.toString() ?? '',
              pictureUrl: user.pictureUrl?.toString() ?? '',
            ),
          );
        } else if (status == AuthenticationStatus.error) {
          _updateState(
            (s) => s.copyWith(
              state: ViewFormState.error,
              errorMessage:
                  "Failed to load profile: ${_authenticationProvider.errorMessage}",
            ),
          );
        }
      });
    } catch (e) {
      _updateState(
        (s) => s.copyWith(
          state: ViewFormState.error,
          errorMessage: "Failed to load data: $e",
        ),
      );
    }
  }

  @override
  void dispose() {
    _statusSubscription.cancel();
    _navigationController.close();
    super.dispose();
  }

  Future<void> logout() async {
    _updateState((s) => s.copyWith(state: ViewFormState.loading));
    await _authenticationProvider.logout();
    _updateState((s) => s.copyWith(state: ViewFormState.initial));
    _navigationController.add('/');
  }

  void fallbackToDefaultPicture() {
    _updateState((s) => s.copyWith(state: ViewFormState.ready, pictureUrl: ''));
    notifyListeners();
  }

  void _updateState(ProfileFormState Function(ProfileFormState) update) {
    _formState = update(_formState);
    notifyListeners();
  }
}
