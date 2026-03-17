import 'package:equatable/equatable.dart';

class DistrictModel extends Equatable {
  final int id;
  final String name;
  final String city;

  const DistrictModel({
    required this.id,
    required this.name,
    required this.city,
  });

  factory DistrictModel.fromJson(Map<String, dynamic> json) => DistrictModel(
        id: json['id'] as int,
        name: json['name'] as String? ?? '',
        city: json['city'] as String? ?? 'İstanbul',
      );

  @override
  List<Object?> get props => [id, name, city];
}
