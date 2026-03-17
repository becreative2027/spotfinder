import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/splash_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/onboarding_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/login_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/register_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/otp_screen.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/venue_bloc.dart';
import 'package:spotfinder_app/features/explore/presentation/screens/home_screen.dart';
import 'package:spotfinder_app/features/explore/presentation/screens/explore_screen.dart';
import 'package:spotfinder_app/features/explore/presentation/screens/search_results_screen.dart';
import 'package:spotfinder_app/features/explore/presentation/screens/venue_detail_screen.dart';
import 'package:spotfinder_app/features/explore/presentation/screens/visits_screen.dart';
import 'package:spotfinder_app/features/favorites/presentation/screens/favorites_screen.dart';
import 'package:spotfinder_app/features/profile/presentation/screens/profile_screen.dart';
import 'package:spotfinder_app/features/reviews/presentation/screens/write_review_screen.dart';
import 'package:spotfinder_app/features/settings/presentation/bloc/settings_bloc.dart';
import 'package:spotfinder_app/features/settings/presentation/screens/settings_screen.dart';
import 'package:spotfinder_app/shared/navigation/main_shell.dart';

final GoRouter appRouter = GoRouter(
  initialLocation: '/',
  routes: [
    GoRoute(
      path: '/',
      builder: (_, __) => const SplashScreen(),
    ),
    GoRoute(
      path: '/onboarding',
      builder: (_, __) => const OnboardingScreen(),
    ),
    GoRoute(
      path: '/login',
      builder: (_, __) => const LoginScreen(),
    ),
    GoRoute(
      path: '/register',
      builder: (_, __) => const RegisterScreen(),
    ),
    GoRoute(
      path: '/otp',
      builder: (context, state) {
        final phone = state.extra as String? ?? '';
        return OtpScreen(phoneNumber: phone);
      },
    ),

    // ─── Shell: Bottom Navigation ──────────────────────────────────────────
    ShellRoute(
      builder: (context, state, child) => BlocProvider(
        create: (_) => VenueBloc(
          searchRepository: ServiceLocator.searchRepository,
          venueRepository: ServiceLocator.venueRepository,
        ),
        child: MainShell(child: child),
      ),
      routes: [
        GoRoute(
          path: '/home',
          builder: (_, __) => const HomeScreen(),
        ),
        GoRoute(
          path: '/explore',
          builder: (_, __) => const ExploreScreen(),
        ),
        GoRoute(
          path: '/favorites',
          builder: (_, __) => const FavoritesScreen(),
        ),
        GoRoute(
          path: '/profile',
          builder: (_, __) => const ProfileScreen(),
        ),
      ],
    ),

    // ─── Full-screen routes (no bottom nav) ───────────────────────────────
    GoRoute(
      path: '/search',
      builder: (_, __) => const SearchResultsScreen(),
    ),
    GoRoute(
      path: '/venue/:id',
      builder: (context, state) {
        final venueId = state.pathParameters['id']!;
        return BlocProvider(
          create: (_) => VenueBloc(
            searchRepository: ServiceLocator.searchRepository,
            venueRepository: ServiceLocator.venueRepository,
          ),
          child: VenueDetailScreen(venueId: venueId),
        );
      },
    ),
    GoRoute(
      path: '/visits',
      builder: (_, __) => const VisitsScreen(),
    ),
    GoRoute(
      path: '/venue/:id/review',
      builder: (context, state) {
        final venueId = state.pathParameters['id']!;
        final venueName = state.extra as String? ?? '';
        return WriteReviewScreen(venueId: venueId, venueName: venueName);
      },
    ),
    GoRoute(
      path: '/settings',
      builder: (_, __) => BlocProvider(
        create: (_) => SettingsBloc()..add(const LoadSettings()),
        child: const SettingsScreen(),
      ),
    ),
  ],
);
