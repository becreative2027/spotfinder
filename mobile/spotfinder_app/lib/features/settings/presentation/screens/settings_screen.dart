import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:spotfinder_app/features/settings/presentation/bloc/settings_bloc.dart';

class SettingsScreen extends StatelessWidget {
  const SettingsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Ayarlar')),
      body: BlocBuilder<SettingsBloc, SettingsState>(
        builder: (context, state) {
          return ListView(
            children: [
              // ─── Dil ───────────────────────────────────────────────────
              const _SectionHeader(title: 'Dil'),
              RadioListTile<String>(
                title: const Text('Türkçe'),
                value: 'tr',
                groupValue: state.locale.languageCode,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ChangeLocale(const Locale('tr'))),
              ),
              RadioListTile<String>(
                title: const Text('English'),
                value: 'en',
                groupValue: state.locale.languageCode,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ChangeLocale(const Locale('en'))),
              ),

              const Divider(),

              // ─── Tema ──────────────────────────────────────────────────
              const _SectionHeader(title: 'Tema'),
              RadioListTile<ThemeMode>(
                title: const Text('Sistem Teması'),
                value: ThemeMode.system,
                groupValue: state.themeMode,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ChangeThemeMode(ThemeMode.system)),
              ),
              RadioListTile<ThemeMode>(
                title: const Text('Açık Tema'),
                value: ThemeMode.light,
                groupValue: state.themeMode,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ChangeThemeMode(ThemeMode.light)),
              ),
              RadioListTile<ThemeMode>(
                title: const Text('Koyu Tema'),
                value: ThemeMode.dark,
                groupValue: state.themeMode,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ChangeThemeMode(ThemeMode.dark)),
              ),

              const Divider(),

              // ─── Bildirimler ───────────────────────────────────────────
              const _SectionHeader(title: 'Bildirimler'),
              SwitchListTile(
                title: const Text('Push Bildirimleri'),
                subtitle:
                    const Text('Yeni mekânlar ve öneriler için bildirim al'),
                value: state.notificationsEnabled,
                onChanged: (v) => context
                    .read<SettingsBloc>()
                    .add(ToggleNotifications(v)),
              ),
            ],
          );
        },
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  final String title;
  const _SectionHeader({required this.title});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(16, 16, 16, 4),
      child: Text(
        title,
        style: Theme.of(context).textTheme.labelLarge?.copyWith(
              color: Theme.of(context).colorScheme.primary,
            ),
      ),
    );
  }
}
