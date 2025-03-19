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
}
