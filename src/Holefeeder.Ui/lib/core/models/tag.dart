class Tag {
  final String tag;
  final int count;

  const Tag({required this.tag, required this.count});

  factory Tag.fromJson(Map<String, dynamic> json) {
    return Tag(tag: json['tag'] as String, count: json['count'] as int);
  }

  Map<String, dynamic> toJson() {
    return {'tag': tag, 'count': count};
  }
}
