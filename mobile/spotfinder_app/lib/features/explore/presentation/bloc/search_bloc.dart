import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:spotfinder_app/features/explore/data/models/concept_tag_model.dart';
import 'package:spotfinder_app/features/explore/data/models/district_model.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/explore/data/repositories/search_repository.dart';
import 'package:spotfinder_app/features/explore/data/repositories/venue_repository.dart';

// ─── Events ─────────────────────────────────────────────────────────────────

abstract class SearchEvent extends Equatable {
  const SearchEvent();
  @override
  List<Object?> get props => [];
}

class LoadFilters extends SearchEvent {
  const LoadFilters();
}

class FilterChanged extends SearchEvent {
  final int? districtId;
  final List<int> tagIds;
  final String sortBy;

  const FilterChanged({
    this.districtId,
    this.tagIds = const [],
    this.sortBy = 'rating',
  });

  @override
  List<Object?> get props => [districtId, tagIds, sortBy];
}

class SearchVenues extends SearchEvent {
  final String? nameQuery;
  final int? districtId;
  final List<int> tagIds;
  final String sortBy;

  const SearchVenues({
    this.nameQuery,
    this.districtId,
    this.tagIds = const [],
    this.sortBy = 'rating',
  });

  @override
  List<Object?> get props => [nameQuery, districtId, tagIds, sortBy];
}

class LoadNextPage extends SearchEvent {
  const LoadNextPage();
}

// ─── States ──────────────────────────────────────────────────────────────────

abstract class SearchState extends Equatable {
  const SearchState();
  @override
  List<Object?> get props => [];
}

class SearchInitial extends SearchState {
  const SearchInitial();
}

class FiltersLoading extends SearchState {
  const FiltersLoading();
}

class FiltersLoaded extends SearchState {
  final List<DistrictModel> districts;
  final List<ConceptTagModel> tags;
  final int? selectedDistrictId;
  final List<int> selectedTagIds;
  final String sortBy;

  const FiltersLoaded({
    required this.districts,
    required this.tags,
    this.selectedDistrictId,
    this.selectedTagIds = const [],
    this.sortBy = 'rating',
  });

  FiltersLoaded copyWith({
    int? selectedDistrictId,
    bool clearDistrict = false,
    List<int>? selectedTagIds,
    String? sortBy,
  }) =>
      FiltersLoaded(
        districts: districts,
        tags: tags,
        selectedDistrictId: clearDistrict ? null : (selectedDistrictId ?? this.selectedDistrictId),
        selectedTagIds: selectedTagIds ?? this.selectedTagIds,
        sortBy: sortBy ?? this.sortBy,
      );

  @override
  List<Object?> get props => [districts, tags, selectedDistrictId, selectedTagIds, sortBy];
}

class SearchLoading extends SearchState {
  final List<VenueModel> previousResults;
  const SearchLoading({this.previousResults = const []});
  @override
  List<Object?> get props => [previousResults];
}

class SearchResultsLoaded extends SearchState {
  final List<VenueModel> venues;
  final bool hasMore;
  final int currentPage;
  final int? districtId;
  final List<int> tagIds;
  final String sortBy;
  final String? nameQuery;

  const SearchResultsLoaded({
    required this.venues,
    required this.hasMore,
    required this.currentPage,
    this.districtId,
    this.tagIds = const [],
    this.sortBy = 'rating',
    this.nameQuery,
  });

  @override
  List<Object?> get props => [venues, hasMore, currentPage, districtId, tagIds, sortBy, nameQuery];
}

class SearchLoadingMore extends SearchResultsLoaded {
  const SearchLoadingMore({
    required super.venues,
    required super.hasMore,
    required super.currentPage,
    super.districtId,
    super.tagIds,
    super.sortBy,
    super.nameQuery,
  });
}

class SearchError extends SearchState {
  final String message;
  const SearchError(this.message);
  @override
  List<Object?> get props => [message];
}

// ─── BLoC ────────────────────────────────────────────────────────────────────

class SearchBloc extends Bloc<SearchEvent, SearchState> {
  final SearchRepository searchRepository;
  final VenueRepository venueRepository;

  SearchBloc({
    required this.searchRepository,
    required this.venueRepository,
  }) : super(const SearchInitial()) {
    on<LoadFilters>(_onLoadFilters);
    on<FilterChanged>(_onFilterChanged);
    on<SearchVenues>(_onSearchVenues);
    on<LoadNextPage>(_onLoadNextPage);
  }

  Future<void> _onLoadFilters(
    LoadFilters event,
    Emitter<SearchState> emit,
  ) async {
    emit(const FiltersLoading());
    try {
      final results = await Future.wait([
        venueRepository.getDistricts(),
        venueRepository.getConceptTags(),
      ]);
      emit(FiltersLoaded(
        districts: results[0] as List<DistrictModel>,
        tags: results[1] as List<ConceptTagModel>,
      ));
    } catch (e) {
      emit(const SearchError('Filtreler yüklenemedi.'));
    }
  }

  void _onFilterChanged(
    FilterChanged event,
    Emitter<SearchState> emit,
  ) {
    if (state is FiltersLoaded) {
      final current = state as FiltersLoaded;
      emit(current.copyWith(
        selectedDistrictId: event.districtId,
        clearDistrict: event.districtId == null,
        selectedTagIds: event.tagIds,
        sortBy: event.sortBy,
      ));
    }
  }

  Future<void> _onSearchVenues(
    SearchVenues event,
    Emitter<SearchState> emit,
  ) async {
    // Use filters from event directly — avoids depending on current BLoC state
    // which may be SearchResultsLoaded (not FiltersLoaded) after a prior search.
    final districtId = event.districtId;
    final tagIds = event.tagIds;
    final sortBy = event.sortBy;
    final nameQuery = event.nameQuery;

    emit(const SearchLoading());
    try {
      final result = await searchRepository.search(
        districtId: districtId,
        conceptTagIds: tagIds,
        sortBy: sortBy,
        page: 1,
        nameQuery: nameQuery,
      );
      emit(SearchResultsLoaded(
        venues: result.items,
        hasMore: result.hasNextPage,
        currentPage: 1,
        districtId: districtId,
        tagIds: tagIds,
        sortBy: sortBy,
        nameQuery: nameQuery,
      ));
    } catch (e) {
      emit(const SearchError('Arama yapılamadı. Lütfen tekrar deneyin.'));
    }
  }

  Future<void> _onLoadNextPage(
    LoadNextPage event,
    Emitter<SearchState> emit,
  ) async {
    if (state is! SearchResultsLoaded) return;
    final current = state as SearchResultsLoaded;
    if (!current.hasMore) return;

    emit(SearchLoadingMore(
      venues: current.venues,
      hasMore: current.hasMore,
      currentPage: current.currentPage,
      districtId: current.districtId,
      tagIds: current.tagIds,
      sortBy: current.sortBy,
      nameQuery: current.nameQuery,
    ));

    try {
      final nextPage = current.currentPage + 1;
      final result = await searchRepository.search(
        districtId: current.districtId,
        conceptTagIds: current.tagIds,
        sortBy: current.sortBy,
        page: nextPage,
        nameQuery: current.nameQuery,
      );
      emit(SearchResultsLoaded(
        venues: [...current.venues, ...result.items],
        hasMore: result.hasNextPage,
        currentPage: nextPage,
        districtId: current.districtId,
        tagIds: current.tagIds,
        sortBy: current.sortBy,
        nameQuery: current.nameQuery,
      ));
    } catch (e) {
      emit(current); // Hata durumunda önceki sonuçları koru
    }
  }
}
