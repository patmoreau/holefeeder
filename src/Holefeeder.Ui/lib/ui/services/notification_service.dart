import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:universal_platform/universal_platform.dart';

class NotificationService {
  static Future<void> show({
    required BuildContext context,
    required String message,
    bool isError = false,
  }) async {
    if (UniversalPlatform.isApple) {
      await showCupertinoDialog(
        context: context,
        builder:
            (context) => CupertinoAlertDialog(
              title: Text(isError ? 'Error' : 'Success'),
              content: Text(message),
              actions: [
                CupertinoDialogAction(
                  child: const Text('OK'),
                  onPressed: () => Navigator.of(context).pop(),
                ),
              ],
            ),
      );
    } else {
      await ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(message))).closed;
    }
  }
}
