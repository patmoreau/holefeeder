import 'package:decimal/decimal.dart';
import 'package:json_annotation/json_annotation.dart';

part 'category_dto.g.dart';

@JsonSerializable()
class CategoryDto {
  final String id;
  final String name;
  final String color;
  final double budgetAmount;
  final bool favorite;
  const CategoryDto({
    required this.id,
    required this.name,
    required this.color,
    required this.budgetAmount,
    required this.favorite,
  });
  factory CategoryDto.fromJson(Map<String, dynamic> json) =>
      _$CategoryDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CategoryDtoToJson(this);
}
