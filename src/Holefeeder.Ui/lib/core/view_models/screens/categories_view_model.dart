import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/core/providers/categories_provider.dart';
import 'package:holefeeder/core/view_models/base_view_model.dart';
import 'package:provider/provider.dart';

class CategoriesViewModel extends BaseViewModel {
  late CategoriesProvider _categoriesProvider;

  late Future<List<Category>> _categoriesFuture;

  Future<List<Category>> get categoriesFuture => _categoriesFuture;

  CategoriesViewModel({required super.context}) {
    _categoriesProvider = Provider.of<CategoriesProvider>(context);

    _categoriesFuture = _categoriesProvider.getCategories();
  }

  Future<void> refreshCategories() async {
    _categoriesFuture = _categoriesProvider.getCategories();
    notifyListeners();
    await _categoriesFuture;
  }
}
