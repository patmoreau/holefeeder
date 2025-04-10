import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:universal_platform/universal_platform.dart';

class ErrorBanner extends StatelessWidget {
  final String message;
  final VoidCallback? onRetry;

  const ErrorBanner({super.key, required String? message, this.onRetry})
    : message = message ?? 'An error occurred.';

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? _buildCupertinoError(context)
        : _buildMaterialError(context);
  }

  Widget _buildCupertinoError(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16.0),
      decoration: BoxDecoration(
        color: CupertinoColors.systemRed.withOpacity(0.1),
        borderRadius: BorderRadius.circular(8.0),
        border: Border.all(color: CupertinoColors.systemRed, width: 1.0),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              const Icon(
                CupertinoIcons.exclamationmark_triangle_fill,
                color: CupertinoColors.systemRed,
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  message,
                  style: const TextStyle(color: CupertinoColors.systemRed),
                ),
              ),
            ],
          ),
          if (onRetry != null) ...[
            const SizedBox(height: 8),
            CupertinoButton(
              padding: EdgeInsets.zero,
              onPressed: onRetry,
              child: const Text('Retry'),
            ),
          ],
        ],
      ),
    );
  }

  Widget _buildMaterialError(BuildContext context) {
    return Container(
      padding: const EdgeInsets.all(16.0),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.errorContainer,
        borderRadius: BorderRadius.circular(8.0),
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Row(
            children: [
              Icon(
                Icons.error_outline,
                color: Theme.of(context).colorScheme.error,
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  message,
                  style: TextStyle(color: Theme.of(context).colorScheme.error),
                ),
              ),
            ],
          ),
          if (onRetry != null) ...[
            const SizedBox(height: 8),
            TextButton(onPressed: onRetry, child: const Text('Retry')),
          ],
        ],
      ),
    );
  }
}
