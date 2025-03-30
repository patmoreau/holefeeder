import 'package:decimal/decimal.dart';
import 'package:intl/intl.dart';

class MakePurchase {
  final DateTime date;
  final Decimal amount;
  final String description;
  final String accountId;
  final String categoryId;
  final List<String> tags;

  const MakePurchase({
    required this.date,
    required this.amount,
    required this.description,
    required this.accountId,
    required this.categoryId,
    required this.tags,
  });

  factory MakePurchase.fromJson(Map<String, dynamic> json) {
    return MakePurchase(
      date: DateTime.parse(json['date'] as String),
      amount: Decimal.parse(json['amount'] as String),
      description: json['description'] as String,
      accountId: json['accountId'] as String,
      categoryId: json['categoryId'] as String,
      tags: List<String>.from(json['tags'] as List),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'date': DateFormat('yyyy-MM-dd').format(date),
      'amount': double.parse(amount.toString()),
      'description': description,
      'accountId': accountId,
      'categoryId': categoryId,
      'tags': tags,
    };
  }
}
