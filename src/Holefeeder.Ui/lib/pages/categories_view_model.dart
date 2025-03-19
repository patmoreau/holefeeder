import 'package:flutter/foundation.dart';
import 'package:holefeeder/models/category.dart' as category;
import 'package:holefeeder/services/categories_service.dart';

class CategoriesViewModel extends ChangeNotifier {
  final CategoriesService categoriesService;
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
