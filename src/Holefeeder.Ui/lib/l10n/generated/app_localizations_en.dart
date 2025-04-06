// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for English (`en`).
class AppLocalizationsEn extends AppLocalizations {
  AppLocalizationsEn([String locale = 'en']) : super(locale);

  @override
  String get holefeederTitle => 'Hello World!';

  @override
  String get welcomeMessage => 'Welcome to our app';

  @override
  String helloUser(String userName) {
    return 'Hello $userName';
  }

  @override
  String get appTitle => 'Holefeeder';
}
