import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/explore/data/repositories/search_repository.dart';
import 'package:spotfinder_app/features/explore/data/repositories/venue_repository.dart';

// ─── Events ─────────────────────────────────────────────────────────────────

abstract class VenueEvent extends Equatable {
  const VenueEvent();
  @override
  List<Object?> get props => [];
}

class LoadFeaturedVenues extends VenueEvent {
  final int count;
  const LoadFeaturedVenues({this.count = 10});
  @override
  List<Object?> get props => [count];
}

class LoadVenueDetail extends VenueEvent {
  final String venueId;
  const LoadVenueDetail(this.venueId);
  @override
  List<Object?> get props => [venueId];
}

// ─── States ──────────────────────────────────────────────────────────────────

abstract class VenueState extends Equatable {
  const VenueState();
  @override
  List<Object?> get props => [];
}

class VenueInitial extends VenueState {
  const VenueInitial();
}

class VenueLoading extends VenueState {
  const VenueLoading();
}

class FeaturedVenuesLoaded extends VenueState {
  final List<VenueModel> venues;
  const FeaturedVenuesLoaded(this.venues);
  @override
  List<Object?> get props => [venues];
}

class VenueDetailLoaded extends VenueState {
  final VenueModel venue;
  const VenueDetailLoaded(this.venue);
  @override
  List<Object?> get props => [venue];
}

class VenueError extends VenueState {
  final String message;
  const VenueError(this.message);
  @override
  List<Object?> get props => [message];
}

class VenueNotFound extends VenueState {
  const VenueNotFound();
}

// ─── BLoC ────────────────────────────────────────────────────────────────────

class VenueBloc extends Bloc<VenueEvent, VenueState> {
  final SearchRepository searchRepository;
  final VenueRepository venueRepository;

  VenueBloc({
    required this.searchRepository,
    required this.venueRepository,
  }) : super(const VenueInitial()) {
    on<LoadFeaturedVenues>(_onLoadFeaturedVenues);
    on<LoadVenueDetail>(_onLoadVenueDetail);
  }

  Future<void> _onLoadFeaturedVenues(
    LoadFeaturedVenues event,
    Emitter<VenueState> emit,
  ) async {
    emit(const VenueLoading());
    try {
      final venues = await searchRepository.getFeatured(count: event.count);
      emit(FeaturedVenuesLoaded(venues));
    } catch (e) {
      emit(VenueError(_mapError(e)));
    }
  }

  Future<void> _onLoadVenueDetail(
    LoadVenueDetail event,
    Emitter<VenueState> emit,
  ) async {
    emit(const VenueLoading());
    try {
      final venue = await venueRepository.getById(event.venueId);
      if (venue == null) {
        emit(const VenueNotFound());
      } else {
        emit(VenueDetailLoaded(venue));
      }
    } catch (e) {
      emit(VenueError(_mapError(e)));
    }
  }

  String _mapError(Object e) {
    return 'Mekân bilgileri yüklenemedi. Lütfen tekrar deneyin.';
  }
}
