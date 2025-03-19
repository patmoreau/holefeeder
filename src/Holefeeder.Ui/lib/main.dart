import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:holefeeder/pages/categories_view_model.dart';
import 'package:holefeeder/router.dart';
import 'package:holefeeder/services/categories_service.dart';
import 'package:provider/provider.dart';
import 'package:holefeeder/services/auth_service.dart';
import 'package:provider/single_child_widget.dart';

import 'holefeeder_app.dart';

const appScheme = 'https';

// Global variable to indicate quick action launch
bool launchedFromQuickAction = false;

Future<void> main() async {
  await dotenv.load(fileName: ".env");
  WidgetsFlutterBinding.ensureInitialized();

  final authService = kIsWeb ? WebAuthService() : MobileAuthService();
  authService.init();
  runApp(
    MultiProvider(
      providers: <SingleChildWidget>[
        Provider<CategoriesService>(
          create: (BuildContext context) => CategoriesService(authService),
        ),
        ChangeNotifierProvider<AuthService>.value(value: authService),
        ChangeNotifierProvider<CategoriesViewModel>(
          create: (BuildContext context) {
            return CategoriesViewModel(Provider.of<CategoriesService>(context, listen: false));
          },
        ),
      ],
      child: HolefeederApp(),
    ),
  );

  // Handle quick action after app initialization
  if (launchedFromQuickAction) {
    router.go('/purchase');
  }
}
