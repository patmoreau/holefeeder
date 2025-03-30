import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:universal_platform/universal_platform.dart';

class PlatformTextField extends StatelessWidget {
  final String? labelText;
  final String initialValue;
  final TextEditingController? controller;
  final String? Function(String?)? validator;
  final ValueChanged<String> onChanged;

  const PlatformTextField({
    super.key,
    required this.initialValue,
    required this.onChanged,
    this.labelText,
    this.controller,
    this.validator,
  });

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? CupertinoTextFormFieldRow(
          prefix: labelText != null ? Text(labelText!) : null,
          placeholder: labelText,
          initialValue: initialValue,
          controller: controller,
          onChanged: onChanged,
          validator: validator,
        )
        : TextFormField(
          decoration: InputDecoration(labelText: labelText),
          initialValue: initialValue,
          controller: controller,
          validator: validator,
          onChanged: onChanged,
        );
  }
}
