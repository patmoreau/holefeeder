import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:universal_platform/universal_platform.dart';

Future<T?> showBaseDialog<T>({
  required BuildContext context,
  required Widget child,
  Widget? fixed,
  VoidCallback? onDismiss,
  bool? useRootNavigator,
  String? routeName,
  Map<String, String>? pathParameters,
  Map<String, dynamic>? queryParameters,
}) {
  return Navigator.of(context, rootNavigator: useRootNavigator ?? true)
      .push<T>(
        PageRouteBuilder<T>(
          pageBuilder: (context, animation, secondaryAnimation) => _DialogWrapper<T>(fixed: fixed, child: child),
          settings: RouteSettings(name: routeName),
          opaque: false,
          barrierDismissible: true,
          barrierColor: Colors.transparent,
        ),
      )
      .then((result) {
        if (onDismiss != null) onDismiss();
        return result;
      });
}

class _DialogWrapper<T> extends StatelessWidget {
  final Widget child;
  final Widget? fixed;

  const _DialogWrapper({required this.child, this.fixed, super.key});

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: <Widget>[
        GestureDetector(
          onTap: () => context.pop(),
          child: Container(
            color:
                UniversalPlatform.isApple
                    ? Colors.black.withAlpha(89) // 0.35 * 255 ≈ 89
                    : Color(0xff665b616e),
          ),
        ),
        child,
        if (fixed != null) fixed!,
      ],
    );
  }
}
