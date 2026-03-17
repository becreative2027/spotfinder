import 'package:equatable/equatable.dart';
import 'package:spotfinder_app/features/auth/data/models/user_model.dart';

abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object?> get props => [];
}

/// Initial state before any auth check is performed.
class AuthInitial extends AuthState {
  const AuthInitial();
}

/// Emitted while an async auth operation is in progress.
class AuthLoading extends AuthState {
  const AuthLoading();
}

/// Emitted when the user is successfully authenticated.
class AuthAuthenticated extends AuthState {
  final UserModel user;

  const AuthAuthenticated({required this.user});

  @override
  List<Object?> get props => [user];
}

/// Emitted when no valid session exists.
class AuthUnauthenticated extends AuthState {
  const AuthUnauthenticated();
}

/// Emitted when an auth operation fails.
class AuthError extends AuthState {
  final String message;

  const AuthError({required this.message});

  @override
  List<Object?> get props => [message];
}

/// Emitted after an OTP has been successfully sent to the user's phone.
class OtpSent extends AuthState {
  final String phoneNumber;

  const OtpSent({required this.phoneNumber});

  @override
  List<Object?> get props => [phoneNumber];
}
