// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'category_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

CategoryDto _$CategoryDtoFromJson(Map<String, dynamic> json) => CategoryDto(
  id: json['id'] as String,
  name: json['name'] as String,
  color: json['color'] as String,
  budgetAmount: (json['budgetAmount'] as num).toDouble(),
  favorite: json['favorite'] as bool,
);

Map<String, dynamic> _$CategoryDtoToJson(CategoryDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
      'color': instance.color,
      'budgetAmount': instance.budgetAmount,
      'favorite': instance.favorite,
    };
