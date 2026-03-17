import 'package:equatable/equatable.dart';

class ConceptTagModel extends Equatable {
  final int id;
  final String nameTr;
  final String nameEn;
  final bool isSystem;
  final bool isActive;

  const ConceptTagModel({
    required this.id,
    required this.nameTr,
    required this.nameEn,
    required this.isSystem,
    required this.isActive,
  });

  factory ConceptTagModel.fromJson(Map<String, dynamic> json) => ConceptTagModel(
        id: json['id'] as int,
        nameTr: json['nameTr'] as String? ?? '',
        nameEn: json['nameEn'] as String? ?? '',
        isSystem: json['isSystem'] as bool? ?? true,
        isActive: json['isActive'] as bool? ?? true,
      );

  @override
  List<Object?> get props => [id, nameTr, nameEn, isSystem, isActive];
}
