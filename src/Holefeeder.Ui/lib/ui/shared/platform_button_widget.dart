import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:universal_platform/universal_platform.dart';

class PlatformButton extends StatelessWidget {
  final VoidCallback? onPressed;
  final bool isLoading;
  final String label;
  final Color? backgroundColor;

  const PlatformButton({
    super.key,
    required this.onPressed,
    this.isLoading = false,
    this.label = 'Submit',
    this.backgroundColor,
  });

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? _buildCupertinoButton(context)
        : _buildMaterialButton(context);
  }

  Widget _buildCupertinoButton(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      child: CupertinoButton.filled(
        onPressed: isLoading ? null : onPressed,
        child:
            isLoading
                ? const CupertinoActivityIndicator(color: CupertinoColors.white)
                : Text(label),
      ),
    );
  }

  Widget _buildMaterialButton(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      child: ElevatedButton(
        onPressed: isLoading ? null : onPressed,
        style: ElevatedButton.styleFrom(
          backgroundColor: backgroundColor,
          padding: const EdgeInsets.symmetric(vertical: 16),
        ),
        child:
            isLoading
                ? const SizedBox(
                  height: 20,
                  width: 20,
                  child: CircularProgressIndicator(strokeWidth: 2),
                )
                : Text(label),
      ),
    );
  }
}
