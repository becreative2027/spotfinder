class ReviewModel {
  final String id;
  final String venueId;
  final String userId;
  final String body;
  final int rating;
  final String status;
  final DateTime createdAt;

  const ReviewModel({
    required this.id,
    required this.venueId,
    required this.userId,
    required this.body,
    required this.rating,
    required this.status,
    required this.createdAt,
  });

  factory ReviewModel.fromJson(Map<String, dynamic> json) => ReviewModel(
        id: json['id'] as String,
        venueId: json['venueId'] as String,
        userId: json['userId'] as String,
        body: json['body'] as String,
        rating: (json['rating'] as num).toInt(),
        status: json['status'] as String? ?? 'pending',
        createdAt: DateTime.parse(json['createdAt'] as String),
      );
}
