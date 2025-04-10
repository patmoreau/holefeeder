enum ViewFormState { initial, loading, ready, error }

class BaseFormState {
  final ViewFormState state;
  final String? errorMessage;

  BaseFormState({this.state = ViewFormState.initial, this.errorMessage});
}
