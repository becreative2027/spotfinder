import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:hive/hive.dart';
import 'package:spotfinder_app/core/constants/storage_keys.dart';

class _OnboardingPage {
  final IconData icon;
  final String title;
  final String subtitle;
  final Color iconColor;
  final Color iconBackground;

  const _OnboardingPage({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.iconColor,
    required this.iconBackground,
  });
}

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen> {
  final PageController _pageController = PageController();
  int _currentPage = 0;

  // TODO: use l10n strings once code generation has been run
  final List<_OnboardingPage> _pages = const [
    _OnboardingPage(
      icon: Icons.location_on_rounded,
      title: 'İstanbul\'u Keşfet',
      subtitle: 'Şehrin en iyi mekanlarını bir arada bul',
      iconColor: Color(0xFF6C63FF),
      iconBackground: Color(0xFFEEEDFF),
    ),
    _OnboardingPage(
      icon: Icons.filter_list_rounded,
      title: 'Konsept Filtrele',
      subtitle:
          'Doğum günü, romantik akşam, manzara... dilediğin konsepti seç',
      iconColor: Color(0xFFFF6584),
      iconBackground: Color(0xFFFFEEF1),
    ),
    _OnboardingPage(
      icon: Icons.star_rounded,
      title: 'Değerlendir ve Paylaş',
      subtitle: 'Gittiğin mekanları puanla, arkadaşlarınla paylaş',
      iconColor: Color(0xFFFFB800),
      iconBackground: Color(0xFFFFF8E1),
    ),
  ];

  Future<void> _onGetStarted() async {
    final box = Hive.box('auth_box');
    await box.put(StorageKeys.isOnboardingDone, true);
    if (!mounted) return;
    context.go('/login');
  }

  void _onSkip() {
    _pageController.jumpToPage(_pages.length - 1);
  }

  void _onNext() {
    if (_currentPage < _pages.length - 1) {
      _pageController.nextPage(
        duration: const Duration(milliseconds: 350),
        curve: Curves.easeInOut,
      );
    } else {
      _onGetStarted();
    }
  }

  Widget _buildDotIndicator() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(_pages.length, (index) {
        final isActive = index == _currentPage;
        return AnimatedContainer(
          duration: const Duration(milliseconds: 300),
          margin: const EdgeInsets.symmetric(horizontal: 4),
          width: isActive ? 24 : 8,
          height: 8,
          decoration: BoxDecoration(
            color: isActive
                ? const Color(0xFF6C63FF)
                : const Color(0xFF6C63FF).withOpacity(0.25),
            borderRadius: BorderRadius.circular(4),
          ),
        );
      }),
    );
  }

  @override
  void dispose() {
    _pageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final isLastPage = _currentPage == _pages.length - 1;

    return Scaffold(
      backgroundColor: Colors.white,
      body: SafeArea(
        child: Column(
          children: [
            // Skip button row
            Align(
              alignment: Alignment.topRight,
              child: Padding(
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                child: TextButton(
                  onPressed: isLastPage ? null : _onSkip,
                  child: Text(
                    'Atla', // TODO: use l10n
                    style: TextStyle(
                      fontFamily: 'Poppins',
                      fontSize: 14,
                      color: isLastPage
                          ? Colors.transparent
                          : const Color(0xFF6C63FF),
                    ),
                  ),
                ),
              ),
            ),

            // PageView
            Expanded(
              child: PageView.builder(
                controller: _pageController,
                itemCount: _pages.length,
                onPageChanged: (index) {
                  setState(() => _currentPage = index);
                },
                itemBuilder: (context, index) {
                  final page = _pages[index];
                  return Padding(
                    padding: const EdgeInsets.symmetric(horizontal: 32),
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        // Icon container
                        Container(
                          width: 140,
                          height: 140,
                          decoration: BoxDecoration(
                            color: page.iconBackground,
                            shape: BoxShape.circle,
                          ),
                          child: Icon(
                            page.icon,
                            size: 72,
                            color: page.iconColor,
                          ),
                        ),
                        const SizedBox(height: 48),
                        // Title
                        Text(
                          page.title,
                          textAlign: TextAlign.center,
                          style: const TextStyle(
                            fontFamily: 'Poppins',
                            fontSize: 26,
                            fontWeight: FontWeight.w700,
                            color: Color(0xFF1A1A2E),
                            height: 1.3,
                          ),
                        ),
                        const SizedBox(height: 16),
                        // Subtitle
                        Text(
                          page.subtitle,
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontFamily: 'Poppins',
                            fontSize: 15,
                            fontWeight: FontWeight.w400,
                            color: Colors.grey.shade500,
                            height: 1.6,
                          ),
                        ),
                      ],
                    ),
                  );
                },
              ),
            ),

            // Bottom controls
            Padding(
              padding:
                  const EdgeInsets.symmetric(horizontal: 32, vertical: 24),
              child: Column(
                children: [
                  _buildDotIndicator(),
                  const SizedBox(height: 32),
                  SizedBox(
                    width: double.infinity,
                    height: 52,
                    child: ElevatedButton(
                      onPressed: _onNext,
                      child: Text(
                        isLastPage
                            ? 'Başla' // TODO: use l10n getStarted
                            : 'İleri', // TODO: use l10n next
                        style: const TextStyle(
                          fontFamily: 'Poppins',
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
