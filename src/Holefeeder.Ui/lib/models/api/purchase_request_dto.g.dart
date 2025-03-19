// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'purchase_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AddItemRequestDTO _$AddItemRequestDTOFromJson(Map<String, dynamic> json) =>
    AddItemRequestDTO(
      name: json['name'] as String,
      description: json['description'] as String,
      url: json['url'] as String,
    );

Map<String, dynamic> _$AddItemRequestDTOToJson(AddItemRequestDTO instance) =>
    <String, dynamic>{
      'name': instance.name,
      'description': instance.description,
      'url': instance.url,
    };
