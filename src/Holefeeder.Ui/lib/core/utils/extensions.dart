extension ListExtensions<T> on List<Map<String, dynamic>> {
  List<T> fromJsonList(T Function(Map<String, dynamic>) fromJson) {
    return map((json) => fromJson(json)).toList();
  }
}
