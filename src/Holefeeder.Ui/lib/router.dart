import 'package:go_router/go_router.dart';
import 'package:holefeeder/pages/home_page.dart';
import 'package:holefeeder/pages/learn_flutter_page.dart';
import 'package:holefeeder/pages/login_page.dart';
import 'package:holefeeder/services/auth_service.dart';
import 'package:provider/provider.dart';

import 'main.dart';

final GoRouter router = GoRouter(
  routes: [
    GoRoute(
      path: '/',
      redirect: (context, state) {
        final authService = Provider.of<AuthService>(context, listen: false);
        if (authService.authStatus == AuthStatus.unauthenticated) {
          return '/login';
        }

        // Conditionally delay initial navigation
        if (launchedFromQuickAction) {
          launchedFromQuickAction = false; // Reset the flag
          Future.delayed(Duration.zero, () {
            router.go('/purchase');
          });
          return null; // Prevent immediate navigation
        }

        return null;
      },
      builder: (context, state) => HomePage(initialIndex: 0),
    ),
    GoRoute(path: '/login', builder: (context, state) => LoginPage()),
    GoRoute(path: '/dashboard', builder: (context, state) => HomePage(initialIndex: 0)),
    GoRoute(path: '/cashflow', builder: (context, state) => HomePage(initialIndex: 1)),
    GoRoute(path: '/profile', builder: (context, state) => HomePage(initialIndex: 2)),
    GoRoute(path: '/purchase', builder: (context, state) => LearnFlutterPage()),
  ],
);
