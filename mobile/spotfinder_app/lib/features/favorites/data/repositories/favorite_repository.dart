import 'package:dio/dio.dart';
import 'package:spotfinder_app/core/constants/api_constants.dart';
import 'package:spotfinder_app/features/favorites/data/models/favorite_model.dart';

class FavoriteRepository {
  final Dio _dio;

  FavoriteRepository({required Dio dio}) : _dio = dio;

  Future<List<FavoriteModel>> getFavorites(String accessToken) async {
    final response = await _dio.get(
      '${ApiConstants.authBaseUrl}/api/v1/users/favorites',
      options: Options(headers: {'Authorization': 'Bearer $accessToken'}),
    );
    return (response.data as List<dynamic>)
        .map((f) => FavoriteModel.fromJson(f as Map<String, dynamic>))
        .toList();
  }

  Future<bool> addFavorite(String venueId, String accessToken) async {
    final response = await _dio.post(
      '${ApiConstants.authBaseUrl}/api/v1/users/favorites/$venueId',
      options: Options(headers: {'Authorization': 'Bearer $accessToken'}),
    );
    return response.statusCode == 200 || response.statusCode == 201;
  }

  Future<bool> removeFavorite(String venueId, String accessToken) async {
    final response = await _dio.delete(
      '${ApiConstants.authBaseUrl}/api/v1/users/favorites/$venueId',
      options: Options(headers: {'Authorization': 'Bearer $accessToken'}),
    );
    return response.statusCode == 204 || response.statusCode == 200;
  }
}
