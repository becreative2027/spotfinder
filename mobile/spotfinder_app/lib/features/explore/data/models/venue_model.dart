import 'package:equatable/equatable.dart';
import 'concept_tag_model.dart';

class VenuePhotoModel extends Equatable {
  final String id;
  final String url;
  final bool isMenuPhoto;
  final int displayOrder;

  const VenuePhotoModel({
    required this.id,
    required this.url,
    required this.isMenuPhoto,
    required this.displayOrder,
  });

  factory VenuePhotoModel.fromJson(Map<String, dynamic> json) => VenuePhotoModel(
        id: json['id'] as String,
        url: json['url'] as String,
        isMenuPhoto: json['isMenuPhoto'] as bool? ?? false,
        displayOrder: json['displayOrder'] as int? ?? 0,
      );

  @override
  List<Object?> get props => [id, url, isMenuPhoto, displayOrder];
}

class VenueModel extends Equatable {
  final String id;
  final String name;
  final String? description;
  final int? districtId;
  final String? districtName;
  final String? address;
  final String? parkingStatus;
  final double? lat;
  final double? lng;
  final double averageRating;
  final int reviewCount;
  final bool isActive;
  final DateTime createdAt;
  final String shareUrl;
  final List<VenuePhotoModel> photos;
  final List<ConceptTagModel> conceptTags;

  const VenueModel({
    required this.id,
    required this.name,
    this.description,
    this.districtId,
    this.districtName,
    this.address,
    this.parkingStatus,
    this.lat,
    this.lng,
    required this.averageRating,
    required this.reviewCount,
    required this.isActive,
    required this.createdAt,
    required this.shareUrl,
    required this.photos,
    required this.conceptTags,
  });

  factory VenueModel.fromJson(Map<String, dynamic> json) => VenueModel(
        id: json['id'] as String,
        name: json['name'] as String,
        description: json['description'] as String?,
        districtId: json['districtId'] as int?,
        districtName: json['districtName'] as String?,
        address: json['address'] as String?,
        parkingStatus: json['parkingStatus'] as String?,
        lat: (json['lat'] as num?)?.toDouble(),
        lng: (json['lng'] as num?)?.toDouble(),
        averageRating: (json['averageRating'] as num?)?.toDouble() ?? 0.0,
        reviewCount: json['reviewCount'] as int? ?? 0,
        isActive: json['isActive'] as bool? ?? true,
        createdAt: DateTime.tryParse(json['createdAt'] as String? ?? '') ?? DateTime.now(),
        shareUrl: json['shareUrl'] as String? ?? '',
        photos: (json['photos'] as List<dynamic>?)
                ?.map((p) => VenuePhotoModel.fromJson(p as Map<String, dynamic>))
                .toList() ??
            [],
        conceptTags: (json['conceptTags'] as List<dynamic>?)
                ?.map((t) => ConceptTagModel.fromJson(t as Map<String, dynamic>))
                .toList() ??
            [],
      );

  /// İlk galeri fotoğrafının URL'i (thumbnail)
  String? get thumbnailUrl {
    final gallery = galleryPhotos;
    return gallery.isNotEmpty ? gallery.first.url : null;
  }

  /// Galeri fotoğrafları (menü fotoğrafları hariç), sıralanmış
  List<VenuePhotoModel> get galleryPhotos {
    final result = photos.where((p) => !p.isMenuPhoto).toList();
    result.sort((a, b) => a.displayOrder.compareTo(b.displayOrder));
    return result;
  }

  /// Menü fotoğrafları, sıralanmış
  List<VenuePhotoModel> get menuPhotos {
    final result = photos.where((p) => p.isMenuPhoto).toList();
    result.sort((a, b) => a.displayOrder.compareTo(b.displayOrder));
    return result;
  }

  /// Otopark durumu Türkçe
  String get parkingStatusText {
    switch (parkingStatus?.toLowerCase()) {
      case 'available':
        return 'Otopark mevcut';
      case 'paid':
        return 'Ücretli otopark';
      case 'none':
        return 'Otopark yok';
      default:
        return parkingStatus ?? 'Bilinmiyor';
    }
  }

  @override
  List<Object?> get props => [id, name, averageRating, reviewCount];
}
