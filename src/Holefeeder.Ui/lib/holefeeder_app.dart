import 'dart:io';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/core/constants/strings.dart';
import 'package:holefeeder/router.dart';
import 'package:quick_actions/quick_actions.dart';

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
    if (!kIsWeb && !Platform.isMacOS) {
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
    // SystemChrome.setPreferredOrientations([
    //   DeviceOrientation.portraitUp,
    // ]);

    return MaterialApp.router(
      title: holefeederTitle,
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
    );
  }
}
