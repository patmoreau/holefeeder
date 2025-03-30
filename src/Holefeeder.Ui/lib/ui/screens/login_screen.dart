import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/enums/authentication_status_enum.dart';
import 'package:holefeeder/ui/shared/widgets.dart';
import 'package:universal_platform/universal_platform.dart';

import '../../core/view_models/screens/login_view_model.dart';
import '../shared/view_model_provider.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<LoginViewModel>(
      model: LoginViewModel(context: context),
      builder:
          (model) =>
              UniversalPlatform.isApple
                  ? _buildForCupertino(model)
                  : _buildForMaterial(model, context),
    );
  }

  Widget _buildForCupertino(LoginViewModel model) => CupertinoPageScaffold(
    navigationBar: CupertinoNavigationBar(
      backgroundColor: CupertinoColors.activeBlue.withOpacity(0.5),
      middle: Text('Login'),
    ),
    child: _buildScreen(model),
  );

  Widget _buildForMaterial(LoginViewModel model, BuildContext context) =>
      Scaffold(appBar: AppBar(), body: _buildScreen(model));

  Widget _buildScreen(LoginViewModel model) => Center(
    child: StreamBuilder(
      stream: model.authenticationStatusStream,
      builder: (builder, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting ||
            snapshot.data == AuthenticationStatus.loading) {
          return HolefeederWidgets.activityIndicator();
        }
        if (snapshot.data == AuthenticationStatus.authenticated) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            GoRouter.of(context).go('/');
          });
          return HolefeederWidgets.activityIndicator();
        }
        return Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            model.errorMessage != null
                ? Text('Error: ${model.errorMessage}')
                : const SizedBox.shrink(),
            HolefeederWidgets.button(
              onPressed: () => model.login(),
              child: Text(model.action),
            ),
          ],
        );
      },
    ),
  );
}
