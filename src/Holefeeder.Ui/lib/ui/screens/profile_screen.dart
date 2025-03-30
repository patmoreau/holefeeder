import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/view_models/screens/profile_view_model.dart';
import 'package:holefeeder/ui/shared/view_model_provider.dart';

import '../../core/enums/authentication_status_enum.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  @override
  Widget build(BuildContext context) {
    return ViewModelProvider<ProfilViewModel>(
      model: ProfilViewModel(context: context),
      builder: (model) {
        return Center(
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

              if (snapshot.data == AuthenticationStatus.unauthenticated) {
                WidgetsBinding.instance.addPostFrameCallback((_) {
                  GoRouter.of(context).go('/');
                });
                return CircularProgressIndicator();
              }
              return Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: <Widget>[
                  CircleAvatar(
                    radius: 75,
                    backgroundColor: Colors.blue,
                    backgroundImage: _getImage(model),
                    onBackgroundImageError: (_, __) {
                      // Fallback: use default asset if the network fails
                      setState(() {
                        model.fallbackToDefaultPicture();
                      });
                    },
                  ),
                  const SizedBox(height: 24),
                  Text('Name: ${model.name}'),
                  const SizedBox(height: 48),
                  ElevatedButton(
                    onPressed: () => model.logout(),
                    child: const Text('Logout'),
                  ),
                ],
              );
            },
          ),
        );
      },
    );
  }

  ImageProvider<Object> _getImage(ProfilViewModel model) {
    if (model.pictureUrl == '') {
      return AssetImage('assets/images/default_profile.png');
    }
    return NetworkImage(model.pictureUrl);
  }
}
