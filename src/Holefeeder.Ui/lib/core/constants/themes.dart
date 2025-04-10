import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

CupertinoThemeData holefeederCupertinoTheme = const CupertinoThemeData(
  applyThemeToAll: true,
  brightness: Brightness.light,
  primaryColor: CupertinoColors.activeBlue,
  textTheme: CupertinoTextThemeData(
    textStyle: TextStyle(
      fontFamily: 'SF Pro Display',
      fontSize: 16.0,
      color: CupertinoColors.black,
    ),
    navTitleTextStyle: TextStyle(
      fontWeight: FontWeight.bold,
      fontSize: 20.0,
      color: CupertinoColors.black,
    ),
    navLargeTitleTextStyle: TextStyle(
      fontWeight: FontWeight.bold,
      fontSize: 34.0,
      color: CupertinoColors.black,
    ),
    actionTextStyle: TextStyle(color: CupertinoColors.activeBlue),
    pickerTextStyle: TextStyle(color: CupertinoColors.black),
    dateTimePickerTextStyle: TextStyle(color: CupertinoColors.black),
  ),
  barBackgroundColor: CupertinoColors.systemGrey6,
  scaffoldBackgroundColor: CupertinoColors.systemBackground,
);

ThemeData holefeederMaterialTheme = ThemeData(
  useMaterial3: true,
  colorScheme: ColorScheme.fromSeed(
    seedColor: Colors.blue,
    brightness: Brightness.light,
  ),
  scaffoldBackgroundColor: Colors.grey[100],
  appBarTheme: const AppBarTheme(
    backgroundColor: Colors.blue,
    foregroundColor: Colors.white,
    titleTextStyle: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
  ),
  textTheme: const TextTheme(
    bodyMedium: TextStyle(fontSize: 16.0),
    titleLarge: TextStyle(fontSize: 22.0, fontWeight: FontWeight.bold),
  ),
  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      backgroundColor: Colors.blue,
      foregroundColor: Colors.white,
      textStyle: const TextStyle(fontSize: 18),
    ),
  ),
);
