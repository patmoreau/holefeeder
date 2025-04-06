import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:holefeeder/l10n/l10n.dart';
import 'package:holefeeder/router.dart';
import 'package:quick_actions/quick_actions.dart';
import 'package:universal_platform/universal_platform.dart';

import 'main.dart';

class HolefeederApp extends StatefulWidget {
  const HolefeederApp({super.key});

  @override
  State<HolefeederApp> createState() => _HolefeederAppState();
}

class _HolefeederAppState extends State<HolefeederApp> {
  final QuickActions quickActions = const QuickActions();

  void _setupQuickActions() {
    quickActions.setShortcutItems(<ShortcutItem>[
      const ShortcutItem(
        type: 'action_purchase',
        localizedTitle: 'Purchase',
        icon: 'purchase_icon',
      ),
    ]);
  }

  @override
  void initState() {
    super.initState();
    if (UniversalPlatform.isMobile) {
      quickActions.initialize((String shortcutType) {
        if (shortcutType == 'action_purchase') {
          launchedFromQuickAction = true;
          if (mounted) {
            router.push('/purchase');
          }
        }
      });
      _setupQuickActions();
    }
  }

  @override
  Widget build(BuildContext context) {
    SystemChrome.setPreferredOrientations([DeviceOrientation.portraitUp]);

    return Builder(
      builder:
          (context) =>
              UniversalPlatform.isApple
                  ? _buildCupertinoApp(context)
                  : _buildMaterialApp(context),
    );
  }

  Widget _buildCupertinoApp(BuildContext context) => CupertinoApp.router(
    onGenerateTitle: (context) => AppLocalizations.of(context).holefeederTitle,
    theme: CupertinoThemeData(
      brightness: Brightness.light,
      applyThemeToAll: true,
    ),
    routerConfig: router,
    localizationsDelegates: const <LocalizationsDelegate<dynamic>>[
      AppLocalizations.delegate,
      GlobalMaterialLocalizations.delegate,
      GlobalWidgetsLocalizations.delegate,
      GlobalCupertinoLocalizations.delegate,
    ],
    supportedLocales: const <Locale>[Locale('en', ''), Locale('fr', '')],
    locale: const Locale('fr', ''),
    builder: (context, child) {
      return Localizations.override(
        context: context,
        delegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        child: child ?? const SizedBox(),
      );
    },
  );

  Widget _buildMaterialApp(BuildContext context) => MaterialApp.router(
    onGenerateTitle: (context) => AppLocalizations.of(context).holefeederTitle,
    theme: ThemeData(
      colorScheme: ColorScheme.fromSeed(seedColor: Colors.blue),
      primarySwatch: Colors.blue,
      visualDensity: VisualDensity.adaptivePlatformDensity,
      appBarTheme: AppBarTheme(backgroundColor: Colors.blue.shade900),
      floatingActionButtonTheme: FloatingActionButtonThemeData(
        backgroundColor: Colors.blue[900],
        foregroundColor: Colors.white,
      ),
    ),
    routerConfig: router,
    localizationsDelegates: const <LocalizationsDelegate<dynamic>>[
      AppLocalizations.delegate,
      GlobalMaterialLocalizations.delegate,
      GlobalWidgetsLocalizations.delegate,
      GlobalCupertinoLocalizations.delegate,
    ],
    supportedLocales: const <Locale>[Locale('en', ''), Locale('fr', '')],
    locale: const Locale('fr', ''),
    builder: (context, child) {
      return Localizations.override(
        context: context,
        delegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        child: child ?? const SizedBox(),
      );
    },
  );
}
