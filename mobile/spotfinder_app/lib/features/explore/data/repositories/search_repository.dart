import 'package:dio/dio.dart';
import 'package:spotfinder_app/core/constants/api_constants.dart';
import 'package:spotfinder_app/features/explore/data/models/paged_result.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';

class SearchRepository {
  final Dio _dio;

  SearchRepository({required Dio dio}) : _dio = dio;

  /// Öne çıkan mekânlar — ana sayfa için
  Future<List<VenueModel>> getFeatured({int count = 10}) async {
    final response = await _dio.get(
      '${ApiConstants.searchBaseUrl}/api/v1/search/venues/featured',
      queryParameters: {'count': count},
    );
    return (response.data as List<dynamic>)
        .map((v) => VenueModel.fromJson(v as Map<String, dynamic>))
        .toList();
  }

  /// Filtreli arama
  Future<PagedResult<VenueModel>> search({
    int? districtId,
    List<int>? conceptTagIds,
    String sortBy = 'rating',
    int page = 1,
    int pageSize = 20,
  }) async {
    final query = <String, dynamic>{
      'sortBy': sortBy,
      'page': page,
      'pageSize': pageSize,
    };
    if (districtId != null) query['districtId'] = districtId;
    if (conceptTagIds != null && conceptTagIds.isNotEmpty) {
      query['conceptTagIds'] = conceptTagIds.join(',');
    }

    final response = await _dio.get(
      '${ApiConstants.searchBaseUrl}/api/v1/search/venues',
      queryParameters: query,
    );
    return PagedResult.fromJson(
      response.data as Map<String, dynamic>,
      VenueModel.fromJson,
    );
  }
}
