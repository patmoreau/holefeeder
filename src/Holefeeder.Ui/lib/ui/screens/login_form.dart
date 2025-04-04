import 'dart:async';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:holefeeder/ui/shared/error_banner.dart';
import 'package:holefeeder/ui/shared/platform_button_widget.dart';

import '../../core/view_models/screens/login_view_model.dart';

class LoginForm extends StatefulWidget {
  final LoginViewModel model;

  const LoginForm({super.key, required this.model});

  @override
  State<LoginForm> createState() => _LoginFormState();
}

class _LoginFormState extends State<LoginForm> {
  late final StreamSubscription<String> _navigationSubscription;

  @override
  void initState() {
    super.initState();
    _navigationSubscription = widget.model.navigationStream.listen((route) {
      if (mounted) {
        context.go(route);
      }
    });
  }

  @override
  void dispose() {
    _navigationSubscription.cancel();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (widget.model.formState.state == ViewFormState.loading) {
      return Center(child: CircularProgressIndicator());
    } else if (widget.model.formState.state == ViewFormState.error) {
      return Center(
        child: ErrorBanner(message: widget.model.formState.errorMessage),
      );
    } else {
      return Center(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [..._buildScreen()],
        ),
      );
    }
  }

  List<Widget> _buildScreen() => [
    PlatformButton(
      onPressed: widget.model.login,
      label: widget.model.loginTitle,
    ),
  ];
}
