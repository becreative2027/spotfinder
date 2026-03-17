import 'package:dio/dio.dart';
import 'package:spotfinder_app/core/constants/api_constants.dart';
import 'package:spotfinder_app/features/explore/data/models/concept_tag_model.dart';
import 'package:spotfinder_app/features/explore/data/models/district_model.dart';
import 'package:spotfinder_app/features/explore/data/models/paged_result.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';

class VenueRepository {
  final Dio _dio;

  VenueRepository({required Dio dio}) : _dio = dio;

  Future<VenueModel?> getById(String id) async {
    final response = await _dio.get(
      '${ApiConstants.venueBaseUrl}/api/v1/venues/$id',
    );
    if (response.statusCode == 404) return null;
    return VenueModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<PagedResult<VenueModel>> getAll({int page = 1, int pageSize = 20}) async {
    final response = await _dio.get(
      '${ApiConstants.venueBaseUrl}/api/v1/venues',
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return PagedResult.fromJson(
      response.data as Map<String, dynamic>,
      VenueModel.fromJson,
    );
  }

  Future<List<DistrictModel>> getDistricts() async {
    final response = await _dio.get(
      '${ApiConstants.venueBaseUrl}/api/v1/districts',
    );
    return (response.data as List<dynamic>)
        .map((d) => DistrictModel.fromJson(d as Map<String, dynamic>))
        .toList();
  }

  Future<List<ConceptTagModel>> getConceptTags() async {
    final response = await _dio.get(
      '${ApiConstants.venueBaseUrl}/api/v1/concept-tags',
    );
    return (response.data as List<dynamic>)
        .map((t) => ConceptTagModel.fromJson(t as Map<String, dynamic>))
        .toList();
  }
}
