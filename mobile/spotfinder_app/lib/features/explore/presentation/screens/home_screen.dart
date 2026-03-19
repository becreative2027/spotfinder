import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/explore/data/models/concept_tag_model.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/venue_bloc.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/search_bloc.dart';
import 'package:spotfinder_app/shared/widgets/venue_card.dart';
import 'package:spotfinder_app/shared/widgets/concept_tag_chip.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  List<ConceptTagModel> _cachedTags = [];

  @override
  void initState() {
    super.initState();
    context.read<VenueBloc>().add(const LoadFeaturedVenues(count: 10));
    final searchState = context.read<SearchBloc>().state;
    if (searchState is FiltersLoaded && searchState.tags.isNotEmpty) {
      _cachedTags = searchState.tags;
    } else {
      context.read<SearchBloc>().add(const LoadFilters());
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      backgroundColor: theme.scaffoldBackgroundColor,
      body: SafeArea(
        child: CustomScrollView(
          slivers: [
            // ─── Header ───────────────────────────────────────────────────
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Row(
                      children: [
                        Expanded(
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                'Merhaba! 👋',
                                style: theme.textTheme.bodyMedium?.copyWith(
                                  color: colorScheme.onSurfaceVariant,
                                ),
                              ),
                              const SizedBox(height: 2),
                              Text(
                                'İstanbul\'u Keşfet',
                                style: theme.textTheme.headlineSmall?.copyWith(
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                            ],
                          ),
                        ),
                        GestureDetector(
                          onTap: () => context.push('/profile'),
                          child: Container(
                            width: 42,
                            height: 42,
                            decoration: BoxDecoration(
                              color: colorScheme.primaryContainer,
                              shape: BoxShape.circle,
                            ),
                            child: Icon(
                              Icons.person_outline,
                              color: colorScheme.onPrimaryContainer,
                            ),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    // Arama kutusu
                    GestureDetector(
                      onTap: () => context.go('/explore'),
                      child: Container(
                        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                        decoration: BoxDecoration(
                          color: colorScheme.surfaceVariant.withOpacity(0.5),
                          borderRadius: BorderRadius.circular(14),
                          border: Border.all(
                            color: colorScheme.outline.withOpacity(0.3),
                          ),
                        ),
                        child: Row(
                          children: [
                            Icon(Icons.search, color: colorScheme.onSurfaceVariant, size: 20),
                            const SizedBox(width: 10),
                            Text(
                              'Mekân, ilçe veya konsept ara...',
                              style: theme.textTheme.bodyMedium?.copyWith(
                                color: colorScheme.onSurfaceVariant,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            ),

            // ─── Konsept Etiketleri ───────────────────────────────────────
            SliverToBoxAdapter(
              child: BlocListener<SearchBloc, SearchState>(
                listener: (context, state) {
                  if (state is FiltersLoaded && state.tags.isNotEmpty) {
                    setState(() => _cachedTags = state.tags);
                  }
                },
                child: _cachedTags.isEmpty
                    ? const SizedBox.shrink()
                    : Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Padding(
                            padding: const EdgeInsets.fromLTRB(20, 24, 20, 12),
                            child: Text(
                              'Konsept Keşfet',
                              style: theme.textTheme.titleMedium?.copyWith(
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                          ),
                          SizedBox(
                            height: 40,
                            child: ListView.separated(
                              scrollDirection: Axis.horizontal,
                              padding: const EdgeInsets.symmetric(horizontal: 20),
                              itemCount: _cachedTags.length,
                              separatorBuilder: (_, __) => const SizedBox(width: 8),
                              itemBuilder: (context, index) {
                                final tag = _cachedTags[index];
                                return ConceptTagChip(
                                  tag: tag,
                                  isSelected: false,
                                  onTap: () {
                                    context.read<SearchBloc>().add(FilterChanged(
                                          tagIds: [tag.id],
                                        ));
                                    context.read<SearchBloc>().add(const SearchVenues());
                                    context.push('/search');
                                  },
                                );
                              },
                            ),
                          ),
                        ],
                      ),
              ),
            ),

            // ─── Öne Çıkan Mekânlar ───────────────────────────────────────
            SliverToBoxAdapter(
              child: Padding(
                padding: const EdgeInsets.fromLTRB(20, 24, 20, 12),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Öne Çıkan Mekânlar',
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    TextButton(
                      onPressed: () => context.go('/explore'),
                      child: const Text('Tümü'),
                    ),
                  ],
                ),
              ),
            ),

            SliverToBoxAdapter(
              child: BlocBuilder<VenueBloc, VenueState>(
                builder: (context, state) {
                  if (state is VenueLoading) {
                    return const SizedBox(
                      height: 260,
                      child: Center(child: CircularProgressIndicator()),
                    );
                  }
                  if (state is VenueError) {
                    return _ErrorWidget(
                      message: state.message,
                      onRetry: () => context
                          .read<VenueBloc>()
                          .add(const LoadFeaturedVenues(count: 10)),
                    );
                  }
                  if (state is FeaturedVenuesLoaded) {
                    if (state.venues.isEmpty) {
                      return const _EmptyFeatured();
                    }
                    return _FeaturedList(venues: state.venues);
                  }
                  return const SizedBox.shrink();
                },
              ),
            ),

            const SliverToBoxAdapter(child: SizedBox(height: 24)),
          ],
        ),
      ),
    );
  }
}

class _FeaturedList extends StatelessWidget {
  final List<VenueModel> venues;
  const _FeaturedList({required this.venues});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 290,
      child: ListView.separated(
        scrollDirection: Axis.horizontal,
        padding: const EdgeInsets.symmetric(horizontal: 20),
        itemCount: venues.length,
        separatorBuilder: (_, __) => const SizedBox(width: 16),
        itemBuilder: (context, index) {
          return SizedBox(
            width: 220,
            child: VenueCard(venue: venues[index]),
          );
        },
      ),
    );
  }
}

class _EmptyFeatured extends StatelessWidget {
  const _EmptyFeatured();

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 200,
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.location_off_outlined,
                size: 48, color: Theme.of(context).colorScheme.onSurfaceVariant),
            const SizedBox(height: 12),
            Text(
              'Henüz öne çıkan mekân yok',
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: Theme.of(context).colorScheme.onSurfaceVariant,
                  ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ErrorWidget extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;
  const _ErrorWidget({required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 180,
      child: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Text(message, style: Theme.of(context).textTheme.bodyMedium),
            const SizedBox(height: 12),
            OutlinedButton(onPressed: onRetry, child: const Text('Tekrar Dene')),
          ],
        ),
      ),
    );
  }
}
