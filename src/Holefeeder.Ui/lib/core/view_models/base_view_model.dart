import 'package:flutter/widgets.dart';
import 'package:provider/provider.dart';

import '../utils/authentication_client.dart';

class BaseViewModel extends ChangeNotifier {
  final BuildContext context;

  late AuthenticationClient authenticationProvider;

  BaseViewModel({required this.context}) {
    authenticationProvider = Provider.of<AuthenticationClient>(context);
  }
}
