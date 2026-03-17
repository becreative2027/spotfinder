import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/splash_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/onboarding_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/login_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/register_screen.dart';
import 'package:spotfinder_app/features/auth/presentation/screens/otp_screen.dart';

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
    GoRoute(
      path: '/home',
      builder: (_, __) => const Scaffold(
        body: Center(
          child: Text(
            "Ana Sayfa - Adım 9'da gelecek",
            style: TextStyle(
              fontSize: 18,
              fontWeight: FontWeight.w500,
            ),
          ),
        ),
      ),
    ),
  ],
);
