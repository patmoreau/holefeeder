import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/core/view_models/screens/profile_view_model.dart';
import 'package:holefeeder/ui/screens/profile_form.dart';
import 'package:holefeeder/ui/shared/view_model_provider.dart';
import 'package:universal_platform/universal_platform.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<ProfileViewModel>(
      model: ProfileViewModel(context: context),
      builder:
          (model) =>
              UniversalPlatform.isApple
                  ? _buildCupertinoScaffold(model)
                  : _buildMaterialScaffold(model),
    );
  }

  Widget _buildCupertinoScaffold(ProfileViewModel model) {
    return CupertinoPageScaffold(
      navigationBar: CupertinoNavigationBar(middle: Text(model.screenTitle)),
      child: SafeArea(child: ProfileForm(model: model)),
    );
  }

  Widget _buildMaterialScaffold(ProfileViewModel model) {
    return Scaffold(
      appBar: AppBar(title: Text(model.screenTitle)),
      body: ProfileForm(model: model),
    );
  }
}
