import 'package:flutter/foundation.dart';
import 'package:holefeeder/core/models/category.dart' as category;
import 'package:holefeeder/core/providers/categories_provider.dart';

class CategoriesViewModel extends ChangeNotifier {
  final CategoriesProvider categoriesService;
  late Future<List<category.Category>> _categoriesFuture;

  Future<List<category.Category>> get categoriesFuture => _categoriesFuture;

  CategoriesViewModel(this.categoriesService);

  Future<void> loadInitialCategories() => _categoriesFuture = categoriesService.getCategories();

  Future<void> refreshCategories() async {
    _categoriesFuture = categoriesService.getCategories();
    notifyListeners();
    await _categoriesFuture;
  }
}
