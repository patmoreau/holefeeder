enum AccountType {
  checking,
  creditCard,
  creditLine,
  investment,
  loan,
  mortgage,
  savings,
}

extension AccountTypeExtension on AccountType {
  static AccountType fromString(String type) {
    return AccountType.values.firstWhere(
      (e) => e.toString().split('.').last.toLowerCase() == type.toLowerCase(),
      orElse: () => throw ArgumentError('Invalid AccountType: $type'),
    );
  }

  String toStringValue() {
    return toString().split('.').last;
  }
}
