import 'package:auth0_flutter/auth0_flutter.dart';
import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/services/auth_service.dart';
import 'package:provider/provider.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
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

        if (authService.authStatus == AuthStatus.unauthenticated) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            GoRouter.of(context).go('/login');
          });
          return Container();
        }
        var profile = authService.userProfile;
        return Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: <Widget>[
              Container(
                width: 150,
                height: 150,
                decoration: BoxDecoration(
                  border: Border.all(color: Colors.blue, width: 4),
                  shape: BoxShape.circle,
                  image: DecorationImage(fit: BoxFit.fill, image: NetworkImage(profile.pictureUrl?.toString() ?? '')),
                ),
              ),
              const SizedBox(height: 24),
              Text('Name: ${profile.name}'),
              const SizedBox(height: 48),
              ElevatedButton(
                onPressed: () async {
                  logout(authService);
                },
                child: const Text('Logout'),
              ),
            ],
          ),
        );
      },
    );
  }

  void logout(AuthService authService) async {
    final router = GoRouter.of(context);

    await authService.logout();

    router.go('/');
  }

  ImageProvider<Object> getImage(UserProfile userProfile) {
    if (userProfile.pictureUrl == null) {
      return AssetImage('assets/images/default_profile.png');
    }
    return NetworkImage(userProfile.pictureUrl?.toString() ?? '');
  }
}
