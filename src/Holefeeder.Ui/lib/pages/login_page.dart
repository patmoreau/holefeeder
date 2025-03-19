import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/services/auth_service.dart';
import 'package:provider/provider.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({super.key});

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  @override
  Widget build(BuildContext context) {
    return Consumer<AuthService>(
      builder: (context, authService, child) {
        if (authService.authStatus == AuthStatus.loading) {
          return Center(child: CircularProgressIndicator());
        }

        if (authService.errorMessage != null) {
          return Text('Error: ${authService.errorMessage}');
        }

        if (authService.authStatus == AuthStatus.authenticated) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            GoRouter.of(context).go('/');
          });
          return Container();
        } else {
          return Scaffold(
            appBar: AppBar(),
            body: Center(
              child: ElevatedButton(
                onPressed: () async {
                  await authService.login();
                },
                child: Text('Login'),
              ),
            ),
          );
        }
      },
    );
  }
}
