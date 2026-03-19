import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_event.dart';
import 'package:spotfinder_app/features/auth/presentation/bloc/auth_state.dart';
import 'package:spotfinder_app/features/favorites/presentation/bloc/favorite_bloc.dart';

class ProfileScreen extends StatelessWidget {
  const ProfileScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colorScheme = theme.colorScheme;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Profilim'),
        actions: [
          IconButton(
            icon: const Icon(Icons.settings_outlined),
            onPressed: () => context.push('/settings'),
          ),
        ],
      ),
      body: BlocBuilder<AuthBloc, AuthState>(
        builder: (context, state) {
          if (state is! AuthAuthenticated) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  const Text('Profili görmek için giriş yapın.'),
                  const SizedBox(height: 16),
                  FilledButton(
                    onPressed: () => context.go('/login'),
                    child: const Text('Giriş Yap'),
                  ),
                ],
              ),
            );
          }

          final user = state.user;
          return SingleChildScrollView(
            padding: const EdgeInsets.all(20),
            child: Column(
              children: [
                const SizedBox(height: 12),
                // Avatar
                CircleAvatar(
                  radius: 48,
                  backgroundColor: colorScheme.primaryContainer,
                  backgroundImage: user.avatarUrl != null
                      ? NetworkImage(user.avatarUrl!)
                      : null,
                  child: user.avatarUrl == null
                      ? Text(
                          (user.fullName?.isNotEmpty == true
                                  ? user.fullName![0]
                                  : user.email[0])
                              .toUpperCase(),
                          style: theme.textTheme.headlineMedium?.copyWith(
                            color: colorScheme.onPrimaryContainer,
                            fontWeight: FontWeight.w700,
                          ),
                        )
                      : null,
                ),
                const SizedBox(height: 16),
                if (user.fullName != null && user.fullName!.isNotEmpty)
                  Text(
                    user.fullName!,
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                const SizedBox(height: 4),
                Text(
                  user.email,
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: colorScheme.onSurfaceVariant,
                  ),
                ),
                const SizedBox(height: 32),

                // Stats
                BlocBuilder<FavoriteBloc, FavoriteState>(
                  builder: (context, favState) {
                    final favCount = favState is FavoriteLoaded
                        ? favState.favoriteIds.length
                        : 0;
                    return Row(
                      children: [
                        Expanded(
                          child: _StatCard(
                            label: 'Favori',
                            value: favCount.toString(),
                            icon: Icons.favorite_rounded,
                            color: Colors.red,
                          ),
                        ),
                        const SizedBox(width: 12),
                        const Expanded(
                          child: _StatCard(
                            label: 'Ziyaret',
                            value: '—',
                            icon: Icons.place_rounded,
                            color: Colors.green,
                          ),
                        ),
                      ],
                    );
                  },
                ),

                const SizedBox(height: 24),
                const Divider(),
                const SizedBox(height: 8),

                // Menu items
                _ProfileMenuItem(
                  icon: Icons.favorite_border_rounded,
                  label: 'Favorilerim',
                  onTap: () => context.go('/favorites'),
                ),
                _ProfileMenuItem(
                  icon: Icons.place_outlined,
                  label: 'Ziyaretlerim',
                  onTap: () => context.push('/visits'),
                ),
                _ProfileMenuItem(
                  icon: Icons.settings_outlined,
                  label: 'Ayarlar',
                  onTap: () => context.push('/settings'),
                ),

                const SizedBox(height: 8),
                const Divider(),
                const SizedBox(height: 8),

                _ProfileMenuItem(
                  icon: Icons.logout_rounded,
                  label: 'Çıkış Yap',
                  color: colorScheme.error,
                  onTap: () async {
                    final confirmed = await showDialog<bool>(
                      context: context,
                      builder: (ctx) => AlertDialog(
                        title: const Text('Çıkış Yap'),
                        content: const Text(
                            'Hesabınızdan çıkış yapmak istediğinize emin misiniz?'),
                        actions: [
                          TextButton(
                            onPressed: () => Navigator.of(ctx).pop(false),
                            child: const Text('Hayır'),
                          ),
                          FilledButton(
                            onPressed: () => Navigator.of(ctx).pop(true),
                            style: FilledButton.styleFrom(
                              backgroundColor: Theme.of(ctx).colorScheme.error,
                            ),
                            child: const Text('Evet, Çıkış Yap'),
                          ),
                        ],
                      ),
                    );
                    if (confirmed == true && context.mounted) {
                      context.read<AuthBloc>().add(const LogoutRequested());
                    }
                  },
                ),
              ],
            ),
          );
        },
      ),
    );
  }
}

class _StatCard extends StatelessWidget {
  final String label;
  final String value;
  final IconData icon;
  final Color color;

  const _StatCard({
    required this.label,
    required this.value,
    required this.icon,
    required this.color,
  });

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 20, horizontal: 16),
        child: Column(
          children: [
            Icon(icon, color: color, size: 28),
            const SizedBox(height: 8),
            Text(
              value,
              style: theme.textTheme.headlineSmall?.copyWith(
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              label,
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _ProfileMenuItem extends StatelessWidget {
  final IconData icon;
  final String label;
  final VoidCallback onTap;
  final Color? color;

  const _ProfileMenuItem({
    required this.icon,
    required this.label,
    required this.onTap,
    this.color,
  });

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;
    final effectiveColor = color ?? colorScheme.onSurface;
    return ListTile(
      leading: Icon(icon, color: effectiveColor),
      title: Text(label, style: TextStyle(color: effectiveColor)),
      trailing: color == null
          ? Icon(Icons.chevron_right, color: colorScheme.onSurfaceVariant)
          : null,
      onTap: onTap,
    );
  }
}
