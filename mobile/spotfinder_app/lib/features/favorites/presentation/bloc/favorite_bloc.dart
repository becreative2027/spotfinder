import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:spotfinder_app/core/constants/storage_keys.dart';
import 'package:spotfinder_app/features/favorites/data/repositories/favorite_repository.dart';

// ─── Events ─────────────────────────────────────────────────────────────────

abstract class FavoriteEvent extends Equatable {
  const FavoriteEvent();
  @override
  List<Object?> get props => [];
}

class LoadFavorites extends FavoriteEvent {
  const LoadFavorites();
}

class ToggleFavorite extends FavoriteEvent {
  final String venueId;
  const ToggleFavorite(this.venueId);
  @override
  List<Object?> get props => [venueId];
}

// ─── States ──────────────────────────────────────────────────────────────────

abstract class FavoriteState extends Equatable {
  const FavoriteState();
  @override
  List<Object?> get props => [];
}

class FavoriteInitial extends FavoriteState {
  const FavoriteInitial();
}

class FavoriteLoading extends FavoriteState {
  const FavoriteLoading();
}

class FavoriteLoaded extends FavoriteState {
  final Set<String> favoriteIds;
  const FavoriteLoaded(this.favoriteIds);

  bool isFavorite(String venueId) => favoriteIds.contains(venueId);

  @override
  List<Object?> get props => [favoriteIds];
}

class FavoriteError extends FavoriteState {
  final String message;
  final Set<String> previousIds;
  const FavoriteError(this.message, this.previousIds);
  @override
  List<Object?> get props => [message, previousIds];
}

// ─── BLoC ────────────────────────────────────────────────────────────────────

class FavoriteBloc extends Bloc<FavoriteEvent, FavoriteState> {
  final FavoriteRepository favoriteRepository;

  FavoriteBloc({required this.favoriteRepository}) : super(const FavoriteInitial()) {
    on<LoadFavorites>(_onLoadFavorites);
    on<ToggleFavorite>(_onToggleFavorite);
  }

  String? get _accessToken {
    final box = Hive.box(StorageKeys.authBox);
    return box.get(StorageKeys.accessToken) as String?;
  }

  Future<void> _onLoadFavorites(
    LoadFavorites event,
    Emitter<FavoriteState> emit,
  ) async {
    final token = _accessToken;
    if (token == null) {
      emit(const FavoriteLoaded({}));
      return;
    }

    emit(const FavoriteLoading());
    try {
      final favorites = await favoriteRepository.getFavorites(token);
      emit(FavoriteLoaded(favorites.map((f) => f.venueId).toSet()));
    } catch (e) {
      emit(const FavoriteLoaded({}));
    }
  }

  Future<void> _onToggleFavorite(
    ToggleFavorite event,
    Emitter<FavoriteState> emit,
  ) async {
    final token = _accessToken;
    if (token == null) return;

    final currentIds = state is FavoriteLoaded
        ? Set<String>.from((state as FavoriteLoaded).favoriteIds)
        : <String>{};

    final isFav = currentIds.contains(event.venueId);

    // Optimistic update
    if (isFav) {
      currentIds.remove(event.venueId);
    } else {
      currentIds.add(event.venueId);
    }
    emit(FavoriteLoaded(currentIds));

    try {
      if (isFav) {
        await favoriteRepository.removeFavorite(event.venueId, token);
      } else {
        await favoriteRepository.addFavorite(event.venueId, token);
      }
    } catch (e) {
      // Hata durumunda geri al
      if (isFav) {
        currentIds.add(event.venueId);
      } else {
        currentIds.remove(event.venueId);
      }
      emit(FavoriteLoaded(currentIds));
    }
  }
}
