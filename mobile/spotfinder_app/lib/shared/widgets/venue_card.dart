import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';

class VenueCard extends StatelessWidget {
  final VenueModel venue;
  final bool showFavoriteButton;

  const VenueCard({
    super.key,
    required this.venue,
    this.showFavoriteButton = true,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return GestureDetector(
      onTap: () => context.push('/venue/${venue.id}'),
      child: Container(
        decoration: BoxDecoration(
          color: theme.cardColor,
          borderRadius: BorderRadius.circular(16),
          boxShadow: [
            BoxShadow(
              color: Colors.black.withOpacity(0.08),
              blurRadius: 12,
              offset: const Offset(0, 4),
            ),
          ],
        ),
        clipBehavior: Clip.antiAlias,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Fotoğraf
            Stack(
              children: [
                _buildImage(context),
                if (showFavoriteButton)
                  Positioned(
                    top: 8,
                    right: 8,
                    child: _FavoriteButton(venueId: venue.id),
                  ),
              ],
            ),
            // İçerik
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    venue.name,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  if (venue.districtName != null) ...[
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        Icon(Icons.location_on_outlined,
                            size: 13, color: colorScheme.onSurfaceVariant),
                        const SizedBox(width: 2),
                        Text(
                          venue.districtName!,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ],
                    ),
                  ],
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      Icon(Icons.star_rounded, size: 15, color: Colors.amber[600]),
                      const SizedBox(width: 3),
                      Text(
                        venue.averageRating.toStringAsFixed(1),
                        style: theme.textTheme.bodySmall?.copyWith(
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(width: 4),
                      Text(
                        '(${venue.reviewCount})',
                        style: theme.textTheme.bodySmall?.copyWith(
                          color: colorScheme.onSurfaceVariant,
                        ),
                      ),
                      const Spacer(),
                      if (venue.conceptTags.isNotEmpty)
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
                          decoration: BoxDecoration(
                            color: colorScheme.primaryContainer,
                            borderRadius: BorderRadius.circular(20),
                          ),
                          child: Text(
                            venue.conceptTags.first.nameTr,
                            style: TextStyle(
                              fontSize: 11,
                              fontWeight: FontWeight.w500,
                              color: colorScheme.onPrimaryContainer,
                            ),
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildImage(BuildContext context) {
    final thumbnail = venue.thumbnailUrl;
    return SizedBox(
      height: 160,
      width: double.infinity,
      child: thumbnail != null
          ? CachedNetworkImage(
              imageUrl: thumbnail,
              fit: BoxFit.cover,
              placeholder: (_, __) => Container(
                color: Theme.of(context).colorScheme.surfaceVariant,
                child: const Center(child: CircularProgressIndicator(strokeWidth: 2)),
              ),
              errorWidget: (_, __, ___) => _placeholderImage(context),
            )
          : _placeholderImage(context),
    );
  }

  Widget _placeholderImage(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    return Container(
      color: colorScheme.surfaceVariant,
      child: Center(
        child: Icon(Icons.image_outlined, size: 48, color: colorScheme.onSurfaceVariant),
      ),
    );
  }
}

class _FavoriteButton extends StatelessWidget {
  final String venueId;
  const _FavoriteButton({required this.venueId});

  @override
  Widget build(BuildContext context) {
    return BlocBuilder<FavoriteBloc, FavoriteState>(
      builder: (context, state) {
        final isFav = state is FavoriteLoaded && state.isFavorite(venueId);
        return GestureDetector(
          onTap: () => context.read<FavoriteBloc>().add(ToggleFavorite(venueId)),
          child: Container(
            width: 34,
            height: 34,
            decoration: BoxDecoration(
              color: Colors.white.withOpacity(0.9),
              shape: BoxShape.circle,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.15),
                  blurRadius: 6,
                ),
              ],
            ),
            child: Icon(
              isFav ? Icons.favorite : Icons.favorite_border,
              size: 18,
              color: isFav ? Colors.red : Colors.grey[600],
            ),
          ),
        );
      },
    );
  }
}
