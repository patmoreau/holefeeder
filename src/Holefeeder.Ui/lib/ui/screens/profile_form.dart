import 'dart:async';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/view_models/base_form_state.dart';
import 'package:holefeeder/core/view_models/screens/profile_view_model.dart';
import 'package:holefeeder/ui/shared/error_banner.dart';
import 'package:holefeeder/ui/shared/widgets.dart';

class ProfileForm extends StatefulWidget {
  final ProfileViewModel model;

  const ProfileForm({super.key, required this.model});

  @override
  State<ProfileForm> createState() => _ProfileFormState();
}

class _ProfileFormState extends State<ProfileForm> {
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
          mainAxisAlignment: MainAxisAlignment.center,
          children: [..._buildScreen()],
        ),
      );
    }
  }

  List<Widget> _buildScreen() => [
    CircleAvatar(
      radius: 75,
      backgroundColor: Colors.blue,
      backgroundImage: _getImage(widget.model),
      onBackgroundImageError: (_, __) {
        setState(() {
          widget.model.fallbackToDefaultPicture();
        });
      },
    ),
    const SizedBox(height: 24),
    Text('Name: ${widget.model.formState.name}'),
    const SizedBox(height: 48),
    HolefeederWidgets.button(
      onPressed: widget.model.logout,
      child: Text(widget.model.logoutTitle),
    ),
  ];

  ImageProvider<Object> _getImage(ProfileViewModel model) {
    if (model.formState.pictureUrl.isEmpty) {
      return AssetImage(model.fallbackPictureUrl);
    }
    return NetworkImage(model.formState.pictureUrl);
  }
}
