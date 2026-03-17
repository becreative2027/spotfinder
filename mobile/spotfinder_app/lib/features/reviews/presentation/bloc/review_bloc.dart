import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:spotfinder_app/core/constants/storage_keys.dart';
import 'package:spotfinder_app/features/reviews/data/models/review_model.dart';
import 'package:spotfinder_app/features/reviews/data/repositories/review_repository.dart';

// ─── Events ──────────────────────────────────────────────────────────────────

abstract class ReviewEvent extends Equatable {
  const ReviewEvent();
  @override
  List<Object?> get props => [];
}

class LoadVenueReviews extends ReviewEvent {
  final String venueId;
  const LoadVenueReviews(this.venueId);
  @override
  List<Object?> get props => [venueId];
}

class SubmitReview extends ReviewEvent {
  final String venueId;
  final String body;
  final int rating;
  const SubmitReview({required this.venueId, required this.body, required this.rating});
  @override
  List<Object?> get props => [venueId, body, rating];
}

// ─── States ──────────────────────────────────────────────────────────────────

abstract class ReviewState extends Equatable {
  const ReviewState();
  @override
  List<Object?> get props => [];
}

class ReviewInitial extends ReviewState {
  const ReviewInitial();
}

class ReviewLoading extends ReviewState {
  const ReviewLoading();
}

class ReviewsLoaded extends ReviewState {
  final List<ReviewModel> reviews;
  const ReviewsLoaded(this.reviews);
  @override
  List<Object?> get props => [reviews];
}

class ReviewSubmitting extends ReviewState {
  const ReviewSubmitting();
}

class ReviewSubmitted extends ReviewState {
  const ReviewSubmitted();
}

class ReviewError extends ReviewState {
  final String message;
  const ReviewError(this.message);
  @override
  List<Object?> get props => [message];
}

// ─── BLoC ────────────────────────────────────────────────────────────────────

class ReviewBloc extends Bloc<ReviewEvent, ReviewState> {
  final ReviewRepository reviewRepository;

  ReviewBloc({required this.reviewRepository}) : super(const ReviewInitial()) {
    on<LoadVenueReviews>(_onLoadVenueReviews);
    on<SubmitReview>(_onSubmitReview);
  }

  String? get _accessToken {
    final box = Hive.box(StorageKeys.authBox);
    return box.get(StorageKeys.accessToken) as String?;
  }

  Future<void> _onLoadVenueReviews(
    LoadVenueReviews event,
    Emitter<ReviewState> emit,
  ) async {
    emit(const ReviewLoading());
    try {
      final reviews = await reviewRepository.getVenueReviews(event.venueId);
      emit(ReviewsLoaded(reviews));
    } catch (e) {
      emit(ReviewError(e.toString()));
    }
  }

  Future<void> _onSubmitReview(
    SubmitReview event,
    Emitter<ReviewState> emit,
  ) async {
    final token = _accessToken;
    if (token == null) {
      emit(const ReviewError('Yorum yazmak için giriş yapmalısınız.'));
      return;
    }

    emit(const ReviewSubmitting());
    try {
      await reviewRepository.createReview(
        venueId: event.venueId,
        body: event.body,
        rating: event.rating,
        token: token,
      );
      emit(const ReviewSubmitted());
    } catch (e) {
      emit(ReviewError(e.toString()));
    }
  }
}
