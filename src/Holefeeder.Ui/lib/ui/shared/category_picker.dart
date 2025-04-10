import 'package:flutter/widgets.dart';
import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/ui/shared/platform_picker_widget.dart';

class CategoryPicker extends StatelessWidget {
  final List<Category> categories;
  final Category? selectedCategory;
  final ValueChanged<Category?> onChanged;

  const CategoryPicker({
    super.key,
    required this.categories,
    required this.selectedCategory,
    required this.onChanged,
  });

  @override
  Widget build(BuildContext context) {
    return PlatformPicker<Category>(
      label: 'Category',
      value: selectedCategory,
      items: categories,
      displayStringFor: (category) => category.name,
      onChanged: onChanged,
      placeholder: 'Select category',
    );
  }
}
