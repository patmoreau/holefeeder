import 'package:json_annotation/json_annotation.dart';

part 'purchase_request_dto.g.dart';

@JsonSerializable()
class AddItemRequestDTO {
  final String name;
  final String description;
  final String url;
  const AddItemRequestDTO({
    required this.name,
    required this.description,
    required this.url,
  });
  factory AddItemRequestDTO.fromJson(Map<String, dynamic> json) =>
      _$AddItemRequestDTOFromJson(json);
  Map<String, dynamic> toJson() => _$AddItemRequestDTOToJson(this);
}
