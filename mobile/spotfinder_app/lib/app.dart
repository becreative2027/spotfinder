import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:flutter_localizations/flutter_localizations.dart';
import 'package:spotfinder_app/l10n/app_localizations.dart';
import 'package:spotfinder_app/core/router/app_router.dart';
import 'package:spotfinder_app/core/theme/app_theme.dart';
import 'package:spotfinder_app/core/di/service_locator.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_state.dart';
import 'package:spotfinder_app/features/explore/presentation/bloc/search_bloc.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';
import 'package:spotfinder_app/features/settings/presentation/bloc/settings_bloc.dart';

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
        BlocProvider(
          create: (_) => SettingsBloc()..add(const LoadSettings()),
        ),
      ],
      child: BlocListener<AuthBloc, AuthState>(
        listener: (context, state) {
          // Reload favorites whenever auth state changes so profile/favorites
          // counts stay in sync without the user manually visiting the tab.
          context.read<FavoriteBloc>().add(const LoadFavorites());
        },
        child: BlocBuilder<SettingsBloc, SettingsState>(
        buildWhen: (prev, curr) =>
            prev.locale != curr.locale || prev.themeMode != curr.themeMode,
        builder: (context, settings) {
          return MaterialApp.router(
            title: 'SpotFinder',
            debugShowCheckedModeBanner: false,
            theme: AppTheme.lightTheme,
            darkTheme: AppTheme.darkTheme,
            themeMode: settings.themeMode,
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
            locale: settings.locale,
          );
        },
        ),
      ),
    );
  }
}
