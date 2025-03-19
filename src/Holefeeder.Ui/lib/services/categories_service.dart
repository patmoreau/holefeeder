import 'dart:convert';

import 'package:decimal/decimal.dart';
import 'package:holefeeder/helpers/constants.dart';
import 'package:holefeeder/models/api/category_dto.dart';
import 'package:holefeeder/models/category.dart';
import 'package:http/http.dart' as http;

import 'auth_service.dart';

class CategoriesService {
  static final String itemsApiUrl = '$serverUrl/api/v2/categories';
  final AuthService authService;
  CategoriesService(this.authService);
  Future<List<Category>> getCategories() async {
    try {
      final String accessToken = authService.credentials.accessToken;
      final http.Response response = await http.get(
        Uri.parse(itemsApiUrl),
        headers: <String, String>{
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $accessToken',
        },
      );
      if (response.statusCode == 200) {
        final List<dynamic> decodedJsonList = jsonDecode(response.body);
        final List<CategoryDto> items = List<CategoryDto>.from(
            decodedJsonList.map((json) => CategoryDto.fromJson(json)));
        return items?.map((CategoryDto dto) => Category(
            id: dto.id,
            name: dto.name,
            color: dto.color,
            budgetAmount: Decimal.parse(dto.budgetAmount.toString()),
            favorite: dto.favorite)).toList() ?? <Category>[];
      }
    } catch (e) {
      throw Exception('Could not get the categories');
    }
    throw Exception('Could not get the categories');
  }
}
