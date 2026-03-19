import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/shared/widgets/venue_card.dart';

class FavoritesScreen extends StatefulWidget {
  const FavoritesScreen({super.key});

  @override
  State<FavoritesScreen> createState() => _FavoritesScreenState();
}

class _FavoritesScreenState extends State<FavoritesScreen> {
  final List<VenueModel> _venues = [];
  bool _isLoading = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    context.read<FavoriteBloc>().add(const LoadFavorites());
  }

  Future<void> _loadVenueDetails(Set<String> venueIds) async {
    if (venueIds.isEmpty) {
      setState(() => _venues.clear());
      return;
    }
    setState(() {
      _isLoading = true;
      _error = null;
    });
    try {
      final repo = ServiceLocator.venueRepository;
      final results = await Future.wait(
        venueIds.map((id) => repo.getById(id)),
      );
      if (mounted) {
        setState(() {
          _venues
            ..clear()
            ..addAll(results.whereType<VenueModel>());
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = 'Favoriler yüklenemedi.';
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Favorilerim')),
      body: BlocListener<FavoriteBloc, FavoriteState>(
        listener: (context, state) {
          if (state is FavoriteLoaded) {
            _loadVenueDetails(state.favoriteIds);
          }
        },
        child: BlocBuilder<FavoriteBloc, FavoriteState>(
          builder: (context, state) {
            if (state is FavoriteLoading) {
              return const Center(child: CircularProgressIndicator());
            }

            if (_isLoading) {
              return const Center(child: CircularProgressIndicator());
            }

            if (_error != null) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(_error!),
                    const SizedBox(height: 12),
                    OutlinedButton(
                      onPressed: () =>
                          context.read<FavoriteBloc>().add(const LoadFavorites()),
                      child: const Text('Tekrar Dene'),
                    ),
                  ],
                ),
              );
            }

            if (_venues.isEmpty && state is FavoriteLoaded) {
              return _EmptyFavorites();
            }

            return ListView.separated(
              padding: const EdgeInsets.all(20),
              itemCount: _venues.length,
              separatorBuilder: (_, __) => const SizedBox(height: 16),
              itemBuilder: (context, index) => VenueCard(venue: _venues[index]),
            );
          },
        ),
      ),
    );
  }
}

class _EmptyFavorites extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(
              Icons.favorite_border,
              size: 64,
              color: theme.colorScheme.onSurfaceVariant,
            ),
            const SizedBox(height: 16),
            Text(
              'Henüz favori eklemediniz',
              style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
            ),
            const SizedBox(height: 8),
            Text(
              'Beğendiğiniz mekânları favorilere ekleyerek buradan kolayca ulaşabilirsiniz.',
              style: theme.textTheme.bodyMedium?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }
}
