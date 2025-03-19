import 'package:decimal/decimal.dart';

class Category {
  final String id;
  final String name;
  final String color;
  final Decimal budgetAmount;
  final bool favorite;

  const Category({
    required this.id,
    required this.name,
    required this.color,
    required this.budgetAmount,
    required this.favorite,
  });

  factory Category.fromJson(Map<String, dynamic> json) {
    return Category(
      id: json['id'] as String,
      name: json['name'] as String,
      color: json['color'] as String,
      budgetAmount: Decimal.parse(json['budgetAmount'].toString()),
      favorite: json['favorite'] as bool,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'color': color,
      'budgetAmount': budgetAmount.toString(),
      'favorite': favorite,
    };
  }
}
