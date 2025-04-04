import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:universal_platform/universal_platform.dart';

class PlatformPicker<T> extends StatelessWidget {
  final String label;
  final T? value;
  final List<T> items;
  final String Function(T) displayStringFor;
  final ValueChanged<T?> onChanged;
  final String? placeholder;

  const PlatformPicker({
    super.key,
    required this.label,
    required this.value,
    required this.items,
    required this.displayStringFor,
    required this.onChanged,
    this.placeholder,
  });

  @override
  Widget build(BuildContext context) {
    return UniversalPlatform.isApple
        ? _buildCupertinoDropdown(context)
        : _buildMaterialDropdown(context);
  }

  Widget _buildCupertinoDropdown(BuildContext context) {
    return CupertinoFormRow(
      prefix: Text(label),
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: () => _showCupertinoPicker(context),
        child: Text(
          value != null
              ? displayStringFor(value as T)
              : (placeholder ?? 'Select...'),
          style: const TextStyle(fontSize: 16),
        ),
      ),
    );
  }

  Widget _buildMaterialDropdown(BuildContext context) {
    return DropdownButtonFormField<T>(
      value: value,
      decoration: InputDecoration(
        labelText: label,
        border: const OutlineInputBorder(),
      ),
      items:
          items.map((T item) {
            return DropdownMenuItem<T>(
              value: item,
              child: Text(displayStringFor(item)),
            );
          }).toList(),
      onChanged: onChanged,
    );
  }

  void _showCupertinoPicker(BuildContext context) {
    showCupertinoModalPopup<void>(
      context: context,
      builder: (BuildContext context) {
        int selectedIndex = value != null ? items.indexOf(value as T) : 0;

        return Container(
          height: 216,
          color: CupertinoColors.systemBackground.resolveFrom(context),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  CupertinoButton(
                    child: const Text('Cancel'),
                    onPressed: () => Navigator.pop(context),
                  ),
                  CupertinoButton(
                    child: const Text('Done'),
                    onPressed: () {
                      onChanged(items[selectedIndex]);
                      Navigator.pop(context);
                    },
                  ),
                ],
              ),
              Expanded(
                child: CupertinoPicker(
                  itemExtent: 32.0,
                  scrollController: FixedExtentScrollController(
                    initialItem: selectedIndex,
                  ),
                  onSelectedItemChanged: (int index) {
                    selectedIndex = index;
                  },
                  children:
                      items.map((item) {
                        return Text(displayStringFor(item));
                      }).toList(),
                ),
              ),
            ],
          ),
        );
      },
    );
  }
}
