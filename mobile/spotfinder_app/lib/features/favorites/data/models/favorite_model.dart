import 'package:equatable/equatable.dart';

class FavoriteModel extends Equatable {
  final String venueId;
  final DateTime createdAt;

  const FavoriteModel({
    required this.venueId,
    required this.createdAt,
  });

  factory FavoriteModel.fromJson(Map<String, dynamic> json) => FavoriteModel(
        venueId: json['venueId'] as String,
        createdAt: DateTime.tryParse(json['createdAt'] as String? ?? '') ?? DateTime.now(),
      );

  @override
  List<Object?> get props => [venueId];
}
