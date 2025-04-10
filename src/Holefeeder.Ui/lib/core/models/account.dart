import 'package:decimal/decimal.dart';
import 'package:holefeeder/core/enums/account_type_enum.dart';

class Account {
  final String id;
  final String name;
  final AccountType type;
  final Decimal openBalance;
  final DateTime openDate;
  final int transactionCount;
  final Decimal balance;
  final DateTime updated;
  final String description;
  final bool favorite;
  final bool inactive;

  const Account({
    required this.id,
    required this.name,
    required this.type,
    required this.openBalance,
    required this.openDate,
    required this.transactionCount,
    required this.balance,
    required this.updated,
    required this.description,
    required this.favorite,
    required this.inactive,
  });

  factory Account.fromJson(Map<String, dynamic> json) {
    return Account(
      id: json['id'] as String,
      name: json['name'] as String,
      type: AccountTypeExtension.fromString(json['type'] as String),
      openBalance: Decimal.parse(json['openBalance'].toString()),
      openDate: DateTime.parse(json['openDate'] as String),
      transactionCount: json['transactionCount'] as int,
      balance: Decimal.parse(json['balance'].toString()),
      updated: DateTime.parse(json['updated'] as String),
      description: json['description'] as String,
      favorite: json['favorite'] as bool,
      inactive: json['inactive'] as bool,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'name': name,
      'type': type.toStringValue(),
      'openBalance': openBalance.toString(),
      'openDate': openDate.toIso8601String(),
      'transactionCount': transactionCount,
      'balance': balance.toString(),
      'updated': updated.toIso8601String(),
      'description': description,
      'favorite': favorite,
      'inactive': inactive,
    };
  }
}
