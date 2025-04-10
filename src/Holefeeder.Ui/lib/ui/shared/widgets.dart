import 'package:decimal/decimal.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:universal_platform/universal_platform.dart';

abstract final class HolefeederWidgets {
  static Widget activityIndicator() =>
      UniversalPlatform.isApple
          ? CupertinoActivityIndicator()
          : CircularProgressIndicator();

  static Widget button({
    required void Function()? onPressed,
    required Widget child,
  }) =>
      UniversalPlatform.isApple
          ? CupertinoButton(onPressed: onPressed, child: child)
          : ElevatedButton(onPressed: onPressed, child: child);

  static Widget textFormField({
    required String labelText,
    TextEditingController? controller,
    String? Function(String?)? validator,
    void Function(String)? onChanged,
    bool showPrefix = true,
  }) =>
      UniversalPlatform.isApple
          ? CupertinoTextFormFieldRow(
            prefix: showPrefix ? Text(labelText) : null,
            placeholder: labelText,
            controller: controller,
            onChanged: onChanged,
            validator: validator,
          )
          : TextFormField(
            decoration: InputDecoration(labelText: labelText),
            controller: controller,
            validator: validator,
            onChanged: onChanged,
          );

  static Widget decimalTextFormField({
    required String labelText,
    void Function(Decimal)? onChanged,
  }) =>
      UniversalPlatform.isApple
          ? CupertinoTextFormFieldRow(
            prefix: Text(labelText),
            textAlign: TextAlign.right,
            keyboardType: TextInputType.numberWithOptions(decimal: true),
            inputFormatters: [_decimalTextInputFormatter],
            placeholder: 'Enter amount',
            onChanged: (value) => _onDecimalChanged(value, onChanged),
            validator: _decimalValidator,
          )
          : TextFormField(
            decoration: InputDecoration(labelText: labelText),
            keyboardType: TextInputType.numberWithOptions(decimal: true),
            inputFormatters: [_decimalTextInputFormatter],
            onChanged: (value) => _onDecimalChanged(value, onChanged),
            validator: _decimalValidator,
          );

  static IconData get iconEdit =>
      UniversalPlatform.isApple
          ? CupertinoIcons.pencil_ellipsis_rectangle
          : Icons.edit;

  static IconData get iconDelete =>
      UniversalPlatform.isApple ? CupertinoIcons.delete : Icons.delete;

  static void showCupertinoDialog(BuildContext context, Widget child) {
    showCupertinoModalPopup<void>(
      context: context,
      builder:
          (BuildContext context) => Container(
            height: 216,
            padding: const EdgeInsets.only(top: 6.0),
            margin: EdgeInsets.only(
              bottom: MediaQuery.of(context).viewInsets.bottom,
            ),
            color: CupertinoColors.systemBackground.resolveFrom(context),
            child: SafeArea(top: false, child: child),
          ),
    );
  }

  static TextInputFormatter get _decimalTextInputFormatter =>
      TextInputFormatter.withFunction((oldValue, newValue) {
        final text = newValue.text;
        // Allow empty text
        if (text.isEmpty) return newValue;
        // Match optional digits, optional decimal, up to two decimals
        final isValid = RegExp(r'^\d*\.?\d{0,2}$').hasMatch(text);
        return isValid ? newValue : oldValue;
      });

  static String? Function(String?)? get _decimalValidator => (String? value) {
    if (value == null || value.isEmpty) {
      return 'Please enter an amount';
    }
    final doubleValue = double.tryParse(value);
    if (doubleValue == null || doubleValue < 0) {
      return 'Please enter a valid amount';
    }
    return null;
  };

  static void _onDecimalChanged(
    String value,
    void Function(Decimal)? callback,
  ) {
    if (value.isEmpty) {
      callback?.call(Decimal.zero);
    } else {
      final decimalValue = Decimal.tryParse(value);
      if (decimalValue != null) {
        callback?.call(decimalValue);
      }
    }
  }
}
