import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/core/view_models/screens/login_view_model.dart';
import 'package:holefeeder/ui/screens/login_form.dart';
import 'package:holefeeder/ui/shared/view_model_provider.dart';
import 'package:universal_platform/universal_platform.dart';

class LoginScreen extends StatelessWidget {
  const LoginScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<LoginViewModel>(
      model: LoginViewModel(context: context),
      builder:
          (model) =>
              UniversalPlatform.isApple
                  ? _buildCupertinoScaffold(model)
                  : _buildMaterialScaffold(model),
    );
  }

  Widget _buildCupertinoScaffold(LoginViewModel model) {
    return CupertinoPageScaffold(
      navigationBar: CupertinoNavigationBar(middle: Text(model.screenTitle)),
      child: SafeArea(child: LoginForm(model: model)),
    );
  }

  Widget _buildMaterialScaffold(LoginViewModel model) {
    return Scaffold(
      appBar: AppBar(title: Text(model.screenTitle)),
      body: LoginForm(model: model),
    );
  }
}
