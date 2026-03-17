import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:spotfinder_app/core/constants/storage_keys.dart';

// ─── Events ──────────────────────────────────────────────────────────────────

abstract class SettingsEvent extends Equatable {
  const SettingsEvent();
  @override
  List<Object?> get props => [];
}

class LoadSettings extends SettingsEvent {
  const LoadSettings();
}

class ChangeLocale extends SettingsEvent {
  final Locale locale;
  const ChangeLocale(this.locale);
  @override
  List<Object?> get props => [locale];
}

class ChangeThemeMode extends SettingsEvent {
  final ThemeMode themeMode;
  const ChangeThemeMode(this.themeMode);
  @override
  List<Object?> get props => [themeMode];
}

class ToggleNotifications extends SettingsEvent {
  final bool enabled;
  const ToggleNotifications(this.enabled);
  @override
  List<Object?> get props => [enabled];
}

// ─── State ───────────────────────────────────────────────────────────────────

class SettingsState extends Equatable {
  final Locale locale;
  final ThemeMode themeMode;
  final bool notificationsEnabled;

  const SettingsState({
    this.locale = const Locale('tr'),
    this.themeMode = ThemeMode.system,
    this.notificationsEnabled = true,
  });

  SettingsState copyWith({
    Locale? locale,
    ThemeMode? themeMode,
    bool? notificationsEnabled,
  }) =>
      SettingsState(
        locale: locale ?? this.locale,
        themeMode: themeMode ?? this.themeMode,
        notificationsEnabled: notificationsEnabled ?? this.notificationsEnabled,
      );

  @override
  List<Object?> get props => [locale, themeMode, notificationsEnabled];
}

// ─── BLoC ────────────────────────────────────────────────────────────────────

class SettingsBloc extends Bloc<SettingsEvent, SettingsState> {
  SettingsBloc() : super(const SettingsState()) {
    on<LoadSettings>(_onLoad);
    on<ChangeLocale>(_onChangeLocale);
    on<ChangeThemeMode>(_onChangeThemeMode);
    on<ToggleNotifications>(_onToggleNotifications);
  }

  Box get _box => Hive.box(StorageKeys.authBox);

  void _onLoad(LoadSettings event, Emitter<SettingsState> emit) {
    final langCode = _box.get('settings_locale', defaultValue: 'tr') as String;
    final themeName = _box.get('settings_theme', defaultValue: 'system') as String;
    final notifs = _box.get('settings_notifications', defaultValue: true) as bool;
    final themeMode = switch (themeName) {
      'light' => ThemeMode.light,
      'dark' => ThemeMode.dark,
      _ => ThemeMode.system,
    };
    emit(SettingsState(
      locale: Locale(langCode),
      themeMode: themeMode,
      notificationsEnabled: notifs,
    ));
  }

  Future<void> _onChangeLocale(
      ChangeLocale event, Emitter<SettingsState> emit) async {
    await _box.put('settings_locale', event.locale.languageCode);
    emit(state.copyWith(locale: event.locale));
  }

  Future<void> _onChangeThemeMode(
      ChangeThemeMode event, Emitter<SettingsState> emit) async {
    final name = switch (event.themeMode) {
      ThemeMode.light => 'light',
      ThemeMode.dark => 'dark',
      _ => 'system',
    };
    await _box.put('settings_theme', name);
    emit(state.copyWith(themeMode: event.themeMode));
  }

  Future<void> _onToggleNotifications(
      ToggleNotifications event, Emitter<SettingsState> emit) async {
    await _box.put('settings_notifications', event.enabled);
    emit(state.copyWith(notificationsEnabled: event.enabled));
  }
}
