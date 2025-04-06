enum DateIntervalType {
  daily,
  weekly,
  monthly,
  yearly,
  oneTime,
}

extension DateIntervalTypeExtension on DateIntervalType {
  static DateIntervalType fromString(String type) {
    return DateIntervalType.values.firstWhere(
      (e) => e.toString().split('.').last.toLowerCase() == type.toLowerCase(),
      orElse: () => throw ArgumentError('Invalid DateIntervalType: $type'),
    );
  }

  String toStringValue() {
    return toString().split('.').last;
  }
}
