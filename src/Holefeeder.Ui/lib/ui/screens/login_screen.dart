import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/enums/authentication_status_enum.dart';

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
      builder: (model) {
        return Scaffold(
          appBar: AppBar(),
          body: Center(
            child: StreamBuilder(
              stream: model.authenticationStatusStream,
              builder: (builder, snapshot) {
                if (snapshot.connectionState == ConnectionState.waiting ||
                    snapshot.data == AuthenticationStatus.loading) {
                  return CircularProgressIndicator();
                }
                if (model.errorMessage != null) {
                  return Text('Error: ${model.errorMessage}');
                }

                if (snapshot.data == AuthenticationStatus.authenticated) {
                  WidgetsBinding.instance.addPostFrameCallback((_) {
                    GoRouter.of(context).go('/');
                  });
                  return CircularProgressIndicator();
                }
                return ElevatedButton(
                  onPressed: () => model.login(),
                  child: Text(model.action),
                );
              },
            ),
          ),
        );
      },
    );
  }
}
