import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:spotfinder_app/l10n/app_localizations.dart';
import 'package:spotfinder_app/core/router/app_router.dart';
import 'package:spotfinder_app/core/theme/app_theme.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/search_bloc.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';

class SpotFinderApp extends StatelessWidget {
  const SpotFinderApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (_) => AuthBloc(authRepository: ServiceLocator.authRepository),
        ),
        // SearchBloc global — hem ShellRoute içinde hem /search ekranında erişilebilir
        BlocProvider(
          create: (_) => SearchBloc(
            searchRepository: ServiceLocator.searchRepository,
            venueRepository: ServiceLocator.venueRepository,
          ),
        ),
        BlocProvider(
          create: (_) => FavoriteBloc(
            favoriteRepository: ServiceLocator.favoriteRepository,
          )..add(const LoadFavorites()),
        ),
      ],
      child: MaterialApp.router(
        title: 'SpotFinder',
        debugShowCheckedModeBanner: false,
        theme: AppTheme.lightTheme,
        darkTheme: AppTheme.darkTheme,
        themeMode: ThemeMode.system,
        routerConfig: appRouter,
        localizationsDelegates: const [
          AppLocalizations.delegate,
          GlobalMaterialLocalizations.delegate,
          GlobalWidgetsLocalizations.delegate,
          GlobalCupertinoLocalizations.delegate,
        ],
        supportedLocales: const [
          Locale('tr'),
          Locale('en'),
        ],
        locale: const Locale('tr'),
      ),
    );
  }
}
