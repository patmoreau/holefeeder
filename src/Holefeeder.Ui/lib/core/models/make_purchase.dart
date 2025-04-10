import 'package:decimal/decimal.dart';
import 'package:intl/intl.dart';

import '../enums/date_interval_type_enum.dart';

class MakePurchase {
  final DateTime date;
  final Decimal amount;
  final String description;
  final String accountId;
  final String categoryId;
  final List<String> tags;
  final CashflowRequest? cashflow;

  const MakePurchase({
    required this.date,
    required this.amount,
    required this.description,
    required this.accountId,
    required this.categoryId,
    required this.tags,
    this.cashflow,
  });

  factory MakePurchase.fromJson(Map<String, dynamic> json) {
    return MakePurchase(
      date: DateTime.parse(json['date'] as String),
      amount: Decimal.parse(json['amount'] as String),
      description: json['description'] as String,
      accountId: json['accountId'] as String,
      categoryId: json['categoryId'] as String,
      tags: List<String>.from(json['tags'] as List),
      cashflow: json['cashflow'] != null
          ? CashflowRequest.fromJson(json['cashflow'] as Map<String, dynamic>)
          : null,
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
      'cashflow': cashflow?.toJson(),
    };
  }
}

class CashflowRequest {
  final DateTime effectiveDate;
  final DateIntervalType intervalType;
  final int frequency;
  final int recurrence;

  const CashflowRequest({
    required this.effectiveDate,
    required this.intervalType,
    required this.frequency,
    required this.recurrence,
  });

  factory CashflowRequest.fromJson(Map<String, dynamic> json) {
    return CashflowRequest(
      effectiveDate: DateTime.parse(json['effectiveDate'] as String),
      intervalType:  DateIntervalTypeExtension.fromString(json['intervalType'] as String),
      frequency: int.parse(json['frequency'] as String),
      recurrence: int.parse(json['recurrence'] as String),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'effectiveDate': DateFormat('yyyy-MM-dd').format(effectiveDate),
      'intervalType': intervalType.toStringValue(),
      'frequency': frequency,
      'recurrence': recurrence,
    };
  }
}
