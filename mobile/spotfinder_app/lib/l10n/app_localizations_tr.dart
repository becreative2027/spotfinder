// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Turkish (`tr`).
class AppLocalizationsTr extends AppLocalizations {
  AppLocalizationsTr([String locale = 'tr']) : super(locale);

  @override
  String get appName => 'SpotFinder';

  @override
  String get login => 'Giriş Yap';

  @override
  String get register => 'Kayıt Ol';

  @override
  String get email => 'E-posta';

  @override
  String get password => 'Şifre';

  @override
  String get fullName => 'Ad Soyad';

  @override
  String get phoneNumber => 'Telefon Numarası';

  @override
  String get continueWithGoogle => 'Google ile devam et';

  @override
  String get continueWithApple => 'Apple ile devam et';

  @override
  String get continueWithPhone => 'Telefon ile devam et';

  @override
  String get dontHaveAccount => 'Hesabınız yok mu?';

  @override
  String get alreadyHaveAccount => 'Zaten hesabınız var mı?';

  @override
  String get forgotPassword => 'Şifremi Unuttum';

  @override
  String get otpTitle => 'Doğrulama Kodu';

  @override
  String get otpSubtitle => 'Telefonunuza gönderilen 6 haneli kodu girin';

  @override
  String get verify => 'Doğrula';

  @override
  String get resendCode => 'Kodu Yeniden Gönder';

  @override
  String get onboardingTitle1 => 'İstanbul\'u Keşfet';

  @override
  String get onboardingSubtitle1 => 'Şehrin en iyi mekanlarını bir arada bul';

  @override
  String get onboardingTitle2 => 'Konsept Filtrele';

  @override
  String get onboardingSubtitle2 =>
      'Doğum günü, romantik akşam, manzara... dilediğin konsepti seç';

  @override
  String get onboardingTitle3 => 'Değerlendir ve Paylaş';

  @override
  String get onboardingSubtitle3 =>
      'Gittiğin mekanları puanla, arkadaşlarınla paylaş';

  @override
  String get getStarted => 'Başla';

  @override
  String get next => 'İleri';

  @override
  String get skip => 'Atla';

  @override
  String get logout => 'Çıkış Yap';

  @override
  String get errorOccurred => 'Bir hata oluştu';

  @override
  String get invalidCredentials => 'E-posta veya şifre hatalı';

  @override
  String get emailRequired => 'E-posta zorunludur';

  @override
  String get passwordRequired => 'Şifre zorunludur';

  @override
  String get nameRequired => 'Ad Soyad zorunludur';

  @override
  String get invalidEmail => 'Geçerli bir e-posta girin';

  @override
  String get passwordTooShort => 'Şifre en az 6 karakter olmalıdır';

  @override
  String get registerSuccess => 'Kayıt başarılı';

  @override
  String get home => 'Ana Sayfa';

  @override
  String get explore => 'Keşfet';

  @override
  String get favorites => 'Favoriler';

  @override
  String get profile => 'Profil';
}
