import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:holefeeder/ui/shared/widgets.dart';
import 'package:intl/intl.dart';
import 'package:universal_platform/universal_platform.dart';

class DatePickerField extends StatelessWidget {
  final DateTime selectedDate;
  final ValueChanged<DateTime> onDateChanged;

  const DatePickerField({
    super.key,
    required this.selectedDate,
    required this.onDateChanged,
  });

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? _buildCupertinoDatePicker(context)
        : _buildMaterialDatePicker(context);
  }

  Widget _buildCupertinoDatePicker(BuildContext context) {
    return CupertinoFormRow(
      prefix: const Text('Date'),
      child: CupertinoButton(
        onPressed:
            () => HolefeederWidgets.showCupertinoDialog(
              context,
              _showCupertinoDatePicker(context),
            ),
        child: Text(DateFormat('yyyy-MM-dd').format(selectedDate)),
      ),
    );
  }

  Widget _buildMaterialDatePicker(BuildContext context) {
    return ListTile(
      title: const Text('Date'),
      subtitle: Text(DateFormat('yyyy-MM-dd').format(selectedDate)),
      onTap: () => _showMaterialDatePicker(context),
    );
  }

  Widget _showCupertinoDatePicker(BuildContext context) {
    return CupertinoDatePicker(
      mode: CupertinoDatePickerMode.date,
      initialDateTime: selectedDate,
      onDateTimeChanged: (date) => onDateChanged(date),
    );
  }

  Future<void> _showMaterialDatePicker(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: selectedDate,
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
      builder: (context, child) {
        return Theme(
          data: Theme.of(context).copyWith(
            colorScheme: Theme.of(context).colorScheme.copyWith(
              primary: Theme.of(context).primaryColor,
              surface: Theme.of(context).colorScheme.surface,
            ),
          ),
          child: child!,
        );
      },
    );

    if (picked != null && picked != selectedDate) {
      onDateChanged(picked);
    }
  }
}
