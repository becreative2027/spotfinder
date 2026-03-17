import 'package:equatable/equatable.dart';

abstract class AuthEvent extends Equatable {
  const AuthEvent();

  @override
  List<Object?> get props => [];
}

/// Fired on app launch to check whether a stored token exists.
class AppStarted extends AuthEvent {
  const AppStarted();
}

/// Fired when the user submits the email/password login form.
class LoginRequested extends AuthEvent {
  final String email;
  final String password;

  const LoginRequested({required this.email, required this.password});

  @override
  List<Object?> get props => [email, password];
}

/// Fired when the user submits the registration form.
class RegisterRequested extends AuthEvent {
  final String fullName;
  final String email;
  final String password;

  const RegisterRequested({
    required this.fullName,
    required this.email,
    required this.password,
  });

  @override
  List<Object?> get props => [fullName, email, password];
}

/// Fired when the user taps "Continue with Google".
class GoogleLoginRequested extends AuthEvent {
  const GoogleLoginRequested();
}

/// Fired when the user requests an OTP to be sent to their phone.
class SendOtpRequested extends AuthEvent {
  final String phoneNumber;

  const SendOtpRequested({required this.phoneNumber});

  @override
  List<Object?> get props => [phoneNumber];
}

/// Fired when the user submits the OTP verification form.
class OtpVerifyRequested extends AuthEvent {
  final String phoneNumber;
  final String code;

  const OtpVerifyRequested({required this.phoneNumber, required this.code});

  @override
  List<Object?> get props => [phoneNumber, code];
}

/// Fired when the user taps "Logout".
class LogoutRequested extends AuthEvent {
  const LogoutRequested();
}
