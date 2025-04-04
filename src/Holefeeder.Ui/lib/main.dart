import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:holefeeder/core/constants/strings.dart';
import 'package:holefeeder/core/enums/authentication_status_enum.dart';
import 'package:holefeeder/core/providers/accounts_provider.dart';
import 'package:holefeeder/core/providers/categories_provider.dart';
import 'package:holefeeder/core/providers/transactions_provider.dart';
import 'package:holefeeder/core/utils/authentication_client.dart';
import 'package:holefeeder/core/utils/rest_client.dart';
import 'package:holefeeder/core/view_models/screens/categories_view_model.dart';
import 'package:holefeeder/router.dart';
import 'package:provider/provider.dart';
import 'package:provider/single_child_widget.dart';

import 'holefeeder_app.dart';

const appScheme = 'https';

// Global variable to indicate quick action launch
bool launchedFromQuickAction = false;

Future<void> main() async {
  await dotenv.load(
    fileName: kReleaseMode ? ".env.production" : ".env.development",
  );
  WidgetsFlutterBinding.ensureInitialized();

  final authenticationService = _createAuthenticationService();
  authenticationService.init();

  runApp(
    MultiProvider(
      providers: <SingleChildWidget>[
        Provider<AuthenticationClient>(
          create: (BuildContext context) => authenticationService,
        ),
        Provider<RestClient>(
          create: (BuildContext context) {
            return RestClient(_createDio(context), baseUrl: serverUrl);
          },
        ),
        Provider<AccountsProvider>(
          create:
              (BuildContext context) => AccountsProvider(
                restClient: Provider.of<RestClient>(context, listen: false),
              ),
        ),
        Provider<CategoriesProvider>(
          create:
              (BuildContext context) => CategoriesProvider(
                restClient: Provider.of<RestClient>(context, listen: false),
              ),
        ),
        Provider<TransactionsProvider>(
          create:
              (BuildContext context) => TransactionsProvider(
                restClient: Provider.of<RestClient>(context, listen: false),
              ),
        ),
        ChangeNotifierProvider<CategoriesViewModel>(
          create: (BuildContext context) {
            return CategoriesViewModel(
              Provider.of<CategoriesProvider>(context, listen: false),
            );
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

AuthenticationClient _createAuthenticationService() =>
    kIsWeb ? WebAuthenticationProvider() : MobileAuthenticationProvider();

Dio _createDio(BuildContext context) {
  final dio = Dio();
  dio.interceptors.add(
    InterceptorsWrapper(
      onRequest: (options, handler) async {
        final authenticationClient = context.read<AuthenticationClient>();

        final status = await authenticationClient.statusStream.first;
        if (status == AuthenticationStatus.authenticated) {
          final token = authenticationClient.credentials.accessToken;
          options.headers['Authorization'] = 'Bearer $token';
        }

        return handler.next(options);
      },
    ),
  );
  return dio;
}
