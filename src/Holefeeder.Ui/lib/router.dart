import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import 'package:holefeeder/core/utils/authentication_client.dart';
import 'package:holefeeder/ui/screens/home_screen.dart';
import 'package:holefeeder/ui/screens/login_screen.dart';
import 'package:holefeeder/ui/screens/purchase_screen.dart';
import 'package:provider/provider.dart';

import 'core/enums/authentication_status_enum.dart';
import 'main.dart';

final GoRouter router = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      redirect: (context, state) async {
        final authenticationClient = Provider.of<AuthenticationClient>(context, listen: false);

        final status = await authenticationClient.statusStream.first;
        if (status != AuthenticationStatus.authenticated) {
          return '/login';
        }

        // Conditionally delay initial navigation
        if (launchedFromQuickAction) {
          launchedFromQuickAction = false; // Reset the flag
          Future.delayed(Duration.zero, () {
            return '/purchase';
          });
          return null; // Prevent immediate navigation
        }

        return null;
      },
      builder: (context, state) => HomeScreen(initialIndex: 0),
    ),
    GoRoute(path: '/login', builder: (context, state) => LoginScreen()),
    GoRoute(path: '/dashboard', builder: (context, state) => HomeScreen(initialIndex: 0)),
    GoRoute(path: '/cashflow', builder: (context, state) => HomeScreen(initialIndex: 1)),
    GoRoute(path: '/profile', builder: (context, state) => HomeScreen(initialIndex: 2)),
    GoRoute(
      path: '/purchase',
      pageBuilder: (context, state) {
        return CupertinoPage(fullscreenDialog: true, child: PurchaseScreen());
      },
    ),
  ],
);
