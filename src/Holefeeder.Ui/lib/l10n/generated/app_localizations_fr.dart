// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for French (`fr`).
class AppLocalizationsFr extends AppLocalizations {
  AppLocalizationsFr([String locale = 'fr']) : super(locale);

  @override
  String get holefeederTitle => 'Bonjour le monde !';

  @override
  String get welcomeMessage => 'Bienvenue dans notre application';

  @override
  String helloUser(String userName) {
    return 'Bonjour $userName';
  }

  @override
  String get appTitle => 'Holefeeder';
}
