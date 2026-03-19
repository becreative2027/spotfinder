import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:share_plus/share_plus.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/features/explore/data/models/venue_model.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/venue_bloc.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_state.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';
import 'package:spotfinder_app/features/reviews/data/models/review_model.dart';
import 'package:spotfinder_app/features/reviews/presentation/bloc/review_bloc.dart';

class VenueDetailScreen extends StatefulWidget {
  final String venueId;
  const VenueDetailScreen({super.key, required this.venueId});

  @override
  State<VenueDetailScreen> createState() => _VenueDetailScreenState();
}

class _VenueDetailScreenState extends State<VenueDetailScreen> {
  int _currentPhotoIndex = 0;

  @override
  void initState() {
    super.initState();
    context.read<VenueBloc>().add(LoadVenueDetail(widget.venueId));
  }

  @override
  Widget build(BuildContext context) {
    return BlocProvider(
      create: (_) => ReviewBloc(reviewRepository: ServiceLocator.reviewRepository),
      child: BlocListener<VenueBloc, VenueState>(
        listener: (context, state) {
          if (state is VenueDetailLoaded) {
            context.read<ReviewBloc>().add(LoadVenueReviews(widget.venueId));
          }
        },
        child: Scaffold(
          body: BlocBuilder<VenueBloc, VenueState>(
            builder: (context, state) {
              if (state is VenueLoading) {
                return const Center(child: CircularProgressIndicator());
              }
              if (state is VenueNotFound) {
                return _NotFound();
              }
              if (state is VenueError) {
                return _ErrorBody(message: state.message, onRetry: () {
                  context.read<VenueBloc>().add(LoadVenueDetail(widget.venueId));
                });
              }
              if (state is VenueDetailLoaded) {
                return _DetailBody(
                  venue: state.venue,
                  currentPhotoIndex: _currentPhotoIndex,
                  onPhotoChanged: (i) => setState(() => _currentPhotoIndex = i),
                );
              }
              return const SizedBox.shrink();
            },
          ),
        ),
      ),
    );
  }
}

class _DetailBody extends StatelessWidget {
  final VenueModel venue;
  final int currentPhotoIndex;
  final ValueChanged<int> onPhotoChanged;

  const _DetailBody({
    required this.venue,
    required this.currentPhotoIndex,
    required this.onPhotoChanged,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;
    final galleryPhotos = venue.galleryPhotos;
    final menuPhotos = venue.menuPhotos;

    return CustomScrollView(
      slivers: [
        // ─── Fotoğraf Galerisi (SliverAppBar) ────────────────────────
        SliverAppBar(
          expandedHeight: 300,
          pinned: true,
          actions: [
            // Favori Butonu
            BlocBuilder<FavoriteBloc, FavoriteState>(
              builder: (context, state) {
                final isFav = state is FavoriteLoaded && state.isFavorite(venue.id);
                return IconButton(
                  icon: Icon(
                    isFav ? Icons.favorite : Icons.favorite_border,
                    color: isFav ? Colors.red : Colors.white,
                  ),
                  onPressed: () {
                    final authState = context.read<AuthBloc>().state;
                    if (authState is! AuthAuthenticated) {
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: const Text('Favorilere eklemek için giriş yapmalısınız.'),
                          action: SnackBarAction(
                            label: 'Giriş Yap',
                            onPressed: () => context.push('/login'),
                          ),
                          behavior: SnackBarBehavior.floating,
                        ),
                      );
                      return;
                    }
                    context.read<FavoriteBloc>().add(ToggleFavorite(venue.id));
                  },
                );
              },
            ),
            // Paylaş Butonu
            IconButton(
              icon: const Icon(Icons.share_outlined, color: Colors.white),
              onPressed: () => Share.share(
                '${venue.name} — ${venue.shareUrl}',
                subject: venue.name,
              ),
            ),
          ],
          flexibleSpace: FlexibleSpaceBar(
            background: galleryPhotos.isEmpty
                ? _PlaceholderHero()
                : _PhotoGallery(
                    photos: galleryPhotos,
                    currentIndex: currentPhotoIndex,
                    onChanged: onPhotoChanged,
                  ),
          ),
        ),

        // ─── İçerik ───────────────────────────────────────────────────
        SliverToBoxAdapter(
          child: Padding(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // İsim ve Puan
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Expanded(
                      child: Text(
                        venue.name,
                        style: theme.textTheme.headlineSmall?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                    ),
                    const SizedBox(width: 12),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Row(
                          children: [
                            Icon(Icons.star_rounded, color: Colors.amber[600], size: 20),
                            const SizedBox(width: 4),
                            Text(
                              venue.averageRating.toStringAsFixed(1),
                              style: theme.textTheme.titleLarge?.copyWith(
                                fontWeight: FontWeight.w700,
                              ),
                            ),
                          ],
                        ),
                        Text(
                          '${venue.reviewCount} yorum',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: colorScheme.onSurfaceVariant,
                          ),
                        ),
                      ],
                    ),
                  ],
                ),

                const SizedBox(height: 12),

                // Konsept Etiketleri
                if (venue.conceptTags.isNotEmpty)
                  Wrap(
                    spacing: 8,
                    runSpacing: 6,
                    children: venue.conceptTags.map((tag) {
                      return Container(
                        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 5),
                        decoration: BoxDecoration(
                          color: colorScheme.primaryContainer,
                          borderRadius: BorderRadius.circular(20),
                        ),
                        child: Text(
                          tag.nameTr,
                          style: TextStyle(
                            fontSize: 12,
                            fontWeight: FontWeight.w500,
                            color: colorScheme.onPrimaryContainer,
                          ),
                        ),
                      );
                    }).toList(),
                  ),

                const SizedBox(height: 20),
                const Divider(),
                const SizedBox(height: 16),

                // Adres
                if (venue.address != null) ...[
                  _InfoRow(
                    icon: Icons.location_on_outlined,
                    text: '${venue.address}${venue.districtName != null ? ', ${venue.districtName}' : ''}',
                  ),
                  const SizedBox(height: 12),
                ],

                // Otopark
                _InfoRow(
                  icon: Icons.local_parking_outlined,
                  text: venue.parkingStatusText,
                ),
                const SizedBox(height: 12),

                // Çalışma Saatleri (placeholder)
                _InfoRow(
                  icon: Icons.access_time_outlined,
                  text: 'Çalışma saatleri bilgisi yakında eklenecek',
                  muted: true,
                ),

                const SizedBox(height: 20),

                // Açıklama
                if (venue.description != null && venue.description!.isNotEmpty) ...[
                  Text(
                    'Hakkında',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    venue.description!,
                    style: theme.textTheme.bodyMedium?.copyWith(height: 1.5),
                  ),
                  const SizedBox(height: 20),
                ],

                // Harita
                if (venue.lat != null && venue.lng != null) ...[
                  Text(
                    'Konum',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  _MiniMap(lat: venue.lat!, lng: venue.lng!, name: venue.name),
                  const SizedBox(height: 12),
                  SizedBox(
                    width: double.infinity,
                    child: OutlinedButton.icon(
                      onPressed: () => _openDirections(venue.lat!, venue.lng!, venue.name),
                      icon: const Icon(Icons.directions_outlined),
                      label: const Text('Yol Tarifi Al'),
                    ),
                  ),
                  const SizedBox(height: 20),
                ],

                // Menü Fotoğrafları
                if (menuPhotos.isNotEmpty) ...[
                  Text(
                    'Menü',
                    style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                  ),
                  const SizedBox(height: 12),
                  SizedBox(
                    height: 180,
                    child: ListView.separated(
                      scrollDirection: Axis.horizontal,
                      itemCount: menuPhotos.length,
                      separatorBuilder: (_, __) => const SizedBox(width: 12),
                      itemBuilder: (context, index) {
                        return GestureDetector(
                          onTap: () => _openFullscreen(context, menuPhotos, index),
                          child: ClipRRect(
                            borderRadius: BorderRadius.circular(12),
                            child: CachedNetworkImage(
                              imageUrl: menuPhotos[index].url,
                              width: 140,
                              height: 180,
                              fit: BoxFit.cover,
                              errorWidget: (_, __, ___) => Container(
                                width: 140,
                                color: colorScheme.surfaceVariant,
                                child: Icon(Icons.broken_image_outlined,
                                    color: colorScheme.onSurfaceVariant),
                              ),
                            ),
                          ),
                        );
                      },
                    ),
                  ),
                  const SizedBox(height: 20),
                ],

                // ─── Yorumlar ──────────────────────────────────────────
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Yorumlar',
                      style: theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.w600),
                    ),
                    TextButton.icon(
                      onPressed: () => context.push(
                        '/venue/${venue.id}/review',
                        extra: venue.name,
                      ),
                      icon: const Icon(Icons.edit_outlined, size: 16),
                      label: const Text('Yorum Yaz'),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                BlocBuilder<ReviewBloc, ReviewState>(
                  builder: (context, state) {
                    if (state is ReviewLoading) {
                      return const Center(child: CircularProgressIndicator());
                    }
                    if (state is ReviewsLoaded) {
                      if (state.reviews.isEmpty) {
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          child: Text(
                            'Henüz yorum yok. İlk yorumu sen yaz!',
                            style: theme.textTheme.bodyMedium?.copyWith(
                              color: colorScheme.onSurfaceVariant,
                            ),
                          ),
                        );
                      }
                      return Column(
                        children: state.reviews
                            .take(5)
                            .map((r) => _ReviewCard(review: r))
                            .toList(),
                      );
                    }
                    return const SizedBox.shrink();
                  },
                ),

                const SizedBox(height: 32),
              ],
            ),
          ),
        ),
      ],
    );
  }

  Future<void> _openDirections(double lat, double lng, String name) async {
    final encodedName = Uri.encodeComponent(name);
    final uri = Uri.parse(
      'https://maps.google.com/?q=$encodedName&ll=$lat,$lng',
    );
    await launchUrl(uri, mode: LaunchMode.externalApplication);
  }
}

class _PhotoGallery extends StatefulWidget {
  final List<VenuePhotoModel> photos;
  final int currentIndex;
  final ValueChanged<int> onChanged;

  const _PhotoGallery({
    required this.photos,
    required this.currentIndex,
    required this.onChanged,
  });

  @override
  State<_PhotoGallery> createState() => _PhotoGalleryState();
}

class _PhotoGalleryState extends State<_PhotoGallery> {
  late final PageController _pageController;

  @override
  void initState() {
    super.initState();
    _pageController = PageController(initialPage: widget.currentIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        PageView.builder(
          controller: _pageController,
          onPageChanged: widget.onChanged,
          itemCount: widget.photos.length,
          itemBuilder: (_, index) => GestureDetector(
            onTap: () => _openFullscreen(context, widget.photos, widget.currentIndex),
            child: CachedNetworkImage(
              imageUrl: widget.photos[index].url,
              fit: BoxFit.cover,
              placeholder: (_, __) => Container(color: Colors.grey[300]),
              errorWidget: (_, __, ___) => Container(
                color: Colors.grey[300],
                child: const Icon(Icons.broken_image_outlined, size: 48),
              ),
            ),
          ),
        ),
        if (widget.photos.length > 1)
          Positioned(
            bottom: 12,
            left: 0,
            right: 0,
            child: Row(
              mainAxisAlignment: MainAxisAlignment.center,
              children: List.generate(widget.photos.length, (index) {
                return AnimatedContainer(
                  duration: const Duration(milliseconds: 200),
                  margin: const EdgeInsets.symmetric(horizontal: 3),
                  width: index == widget.currentIndex ? 20 : 6,
                  height: 6,
                  decoration: BoxDecoration(
                    color: index == widget.currentIndex
                        ? Colors.white
                        : Colors.white.withOpacity(0.5),
                    borderRadius: BorderRadius.circular(3),
                  ),
                );
              }),
            ),
          ),
        // Fullscreen hint icon
        Positioned(
          top: 12,
          right: 12,
          child: Container(
            padding: const EdgeInsets.all(6),
            decoration: BoxDecoration(
              color: Colors.black.withOpacity(0.4),
              borderRadius: BorderRadius.circular(8),
            ),
            child: const Icon(Icons.fullscreen, color: Colors.white, size: 20),
          ),
        ),
      ],
    );
  }
}

void _openFullscreen(BuildContext context, List<VenuePhotoModel> photos, int initialIndex) {
  Navigator.of(context).push(MaterialPageRoute(
    fullscreenDialog: true,
    builder: (_) => _FullscreenPhotoViewer(photos: photos, initialIndex: initialIndex),
  ));
}

class _FullscreenPhotoViewer extends StatefulWidget {
  final List<VenuePhotoModel> photos;
  final int initialIndex;

  const _FullscreenPhotoViewer({required this.photos, required this.initialIndex});

  @override
  State<_FullscreenPhotoViewer> createState() => _FullscreenPhotoViewerState();
}

class _FullscreenPhotoViewerState extends State<_FullscreenPhotoViewer> {
  late PageController _pageController;
  late int _currentIndex;

  @override
  void initState() {
    super.initState();
    _currentIndex = widget.initialIndex;
    _pageController = PageController(initialPage: widget.initialIndex);
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.black,
      appBar: AppBar(
        backgroundColor: Colors.black,
        foregroundColor: Colors.white,
        title: Text(
          '${_currentIndex + 1} / ${widget.photos.length}',
          style: const TextStyle(color: Colors.white),
        ),
      ),
      body: PageView.builder(
        controller: _pageController,
        onPageChanged: (i) => setState(() => _currentIndex = i),
        itemCount: widget.photos.length,
        itemBuilder: (_, index) => _ZoomablePhoto(url: widget.photos[index].url),
      ),
    );
  }
}

/// A single zoomable photo that supports pinch-to-zoom and double-tap-to-zoom.
class _ZoomablePhoto extends StatefulWidget {
  final String url;
  const _ZoomablePhoto({required this.url});

  @override
  State<_ZoomablePhoto> createState() => _ZoomablePhotoState();
}

class _ZoomablePhotoState extends State<_ZoomablePhoto> {
  final TransformationController _controller = TransformationController();
  TapDownDetails? _doubleTapDetails;

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  void _onDoubleTap() {
    if (_controller.value != Matrix4.identity()) {
      _controller.value = Matrix4.identity();
      return;
    }
    final position = _doubleTapDetails?.localPosition ?? Offset.zero;
    const double scale = 2.5;
    _controller.value =
        Matrix4.translationValues(-position.dx * (scale - 1), -position.dy * (scale - 1), 0) *
        Matrix4.diagonal3Values(scale, scale, 1.0);
  }

  @override
  Widget build(BuildContext context) {
    return GestureDetector(
      onDoubleTapDown: (d) => _doubleTapDetails = d,
      onDoubleTap: _onDoubleTap,
      child: InteractiveViewer(
        transformationController: _controller,
        minScale: 1.0,
        maxScale: 4.0,
        child: Center(
          child: CachedNetworkImage(
            imageUrl: widget.url,
            fit: BoxFit.contain,
            placeholder: (_, __) => const Center(
              child: CircularProgressIndicator(color: Colors.white),
            ),
            errorWidget: (_, __, ___) => const Icon(
              Icons.broken_image_outlined,
              color: Colors.white54,
              size: 64,
            ),
          ),
        ),
      ),
    );
  }
}

class _MiniMap extends StatelessWidget {
  final double lat;
  final double lng;
  final String name;

  const _MiniMap({required this.lat, required this.lng, required this.name});

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: 200,
      child: ClipRRect(
        borderRadius: BorderRadius.circular(14),
        child: GoogleMap(
          initialCameraPosition: CameraPosition(
            target: LatLng(lat, lng),
            zoom: 15,
          ),
          markers: {
            Marker(
              markerId: const MarkerId('venue'),
              position: LatLng(lat, lng),
              infoWindow: InfoWindow(title: name),
            ),
          },
          zoomControlsEnabled: false,
          myLocationButtonEnabled: false,
          mapToolbarEnabled: false,
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  final IconData icon;
  final String text;
  final bool muted;

  const _InfoRow({required this.icon, required this.text, this.muted = false});

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    final color = muted ? colorScheme.onSurfaceVariant : colorScheme.onSurface;
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(icon, size: 18, color: color),
        const SizedBox(width: 10),
        Expanded(
          child: Text(
            text,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(color: color),
          ),
        ),
      ],
    );
  }
}

class _ReviewCard extends StatelessWidget {
  final ReviewModel review;
  const _ReviewCard({required this.review});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      child: Padding(
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Row(
                  children: List.generate(5, (i) => Icon(
                    i < review.rating ? Icons.star_rounded : Icons.star_border_rounded,
                    size: 16,
                    color: Colors.amber,
                  )),
                ),
                const Spacer(),
                Text(
                  _formatDate(review.createdAt),
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: theme.colorScheme.onSurfaceVariant,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(review.body, style: theme.textTheme.bodyMedium),
          ],
        ),
      ),
    );
  }

  String _formatDate(DateTime date) {
    return '${date.day}.${date.month.toString().padLeft(2, '0')}.${date.year}';
  }
}

class _PlaceholderHero extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    return Container(
      color: colorScheme.surfaceVariant,
      child: Center(
        child: Icon(Icons.image_outlined, size: 64, color: colorScheme.onSurfaceVariant),
      ),
    );
  }
}

class _NotFound extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const Icon(Icons.search_off, size: 64),
          const SizedBox(height: 16),
          const Text('Mekân bulunamadı'),
          const SizedBox(height: 16),
          OutlinedButton(
            onPressed: () => context.pop(),
            child: const Text('Geri Dön'),
          ),
        ],
      ),
    );
  }
}

class _ErrorBody extends StatelessWidget {
  final String message;
  final VoidCallback onRetry;

  const _ErrorBody({required this.message, required this.onRetry});

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Text(message, textAlign: TextAlign.center),
          const SizedBox(height: 16),
          OutlinedButton(onPressed: onRetry, child: const Text('Tekrar Dene')),
        ],
      ),
    );
  }
}
