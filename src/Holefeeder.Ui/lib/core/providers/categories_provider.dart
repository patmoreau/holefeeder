import 'package:holefeeder/core/models/category.dart';
import 'package:holefeeder/core/providers/base_provider.dart';

class CategoriesProvider extends BaseProvider {
  CategoriesProvider({required super.restClient});

  Future<List<Category>> getCategories() async {
    try {
      final result = await restClient.getCategories();
      if (result.response.statusCode == 200) {
        return result.data;
      }
      throw Exception('Could not get the categories');
    } catch (e) {
      throw Exception('Could not get the categories');
    }
  }

  Future<Category> getCategory(String id) async {
    try {
      final result = await restClient.getCategory(id);
      if (result.response.statusCode == 200) {
        return result.data;
      }
      if (result.response.statusCode == 404) {
        throw Exception('Category not found');
      }
      throw Exception('Could not get the categories');
    } catch (e) {
      throw Exception('Could not get the categories');
    }
  }
}
