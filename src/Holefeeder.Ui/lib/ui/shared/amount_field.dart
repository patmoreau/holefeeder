import 'package:decimal/decimal.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:universal_platform/universal_platform.dart';

class AmountField extends StatelessWidget {
  final Decimal initialValue;
  final ValueChanged<Decimal> onChanged;

  const AmountField({
    super.key,
    required this.initialValue,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? CupertinoTextFormFieldRow(
          prefix: Text('Amount'),
          textAlign: TextAlign.right,
          keyboardType: TextInputType.numberWithOptions(decimal: true),
          inputFormatters: [_decimalTextInputFormatter()],
          initialValue: initialValue.toString(),
          placeholder: 'Enter amount',
          onChanged: (value) => _onDecimalChanged(value, onChanged),
          validator: _decimalValidator(),
        )
        : TextFormField(
          decoration: InputDecoration(labelText: 'Amount'),
          keyboardType: TextInputType.numberWithOptions(decimal: true),
          inputFormatters: [_decimalTextInputFormatter()],
          initialValue: initialValue.toString(),
          onChanged: (value) => _onDecimalChanged(value, onChanged),
          validator: _decimalValidator(),
        );
  }
}

TextInputFormatter _decimalTextInputFormatter() =>
    TextInputFormatter.withFunction((oldValue, newValue) {
      final text = newValue.text;
      // Allow empty text
      if (text.isEmpty) return newValue;
      // Match optional digits, optional decimal, up to two decimals
      final isValid = RegExp(r'^\d*\.?\d{0,2}$').hasMatch(text);
      return isValid ? newValue : oldValue;
    });

String? Function(String?)? _decimalValidator() => (String? value) {
  if (value == null || value.isEmpty) {
    return 'Please enter an amount';
  }
  final doubleValue = double.tryParse(value);
  if (doubleValue == null || doubleValue < 0) {
    return 'Please enter a valid amount';
  }
  return null;
};

void _onDecimalChanged(String value, void Function(Decimal)? callback) {
  if (value.isEmpty) {
    callback?.call(Decimal.zero);
  } else {
    final decimalValue = Decimal.tryParse(value);
    if (decimalValue != null) {
      callback?.call(decimalValue);
    }
  }
}
