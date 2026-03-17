import 'package:dio/dio.dart';
import 'package:spotfinder_app/core/constants/api_constants.dart';
import 'package:spotfinder_app/features/reviews/data/models/review_model.dart';

class ReviewRepository {
  final Dio _dio;

  ReviewRepository({required Dio dio}) : _dio = dio;

  Future<List<ReviewModel>> getVenueReviews(String venueId) async {
    final response = await _dio.get(
      '${ApiConstants.reviewBaseUrl}/api/v1/reviews/venue/$venueId',
    );
    final List<dynamic> data = response.data as List<dynamic>;
    return data
        .map((e) => ReviewModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<ReviewModel> createReview({
    required String venueId,
    required String body,
    required int rating,
    required String token,
  }) async {
    final response = await _dio.post(
      '${ApiConstants.reviewBaseUrl}/api/v1/reviews',
      data: {'venueId': venueId, 'body': body, 'rating': rating},
      options: Options(headers: {'Authorization': 'Bearer $token'}),
    );
    return ReviewModel.fromJson(response.data as Map<String, dynamic>);
  }
}
