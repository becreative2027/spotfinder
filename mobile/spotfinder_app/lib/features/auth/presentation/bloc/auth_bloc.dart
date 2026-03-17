import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:google_sign_in/google_sign_in.dart';
import 'package:spotfinder_app/features/auth/data/repositories/auth_repository.dart';
import 'auth_event.dart';
import 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final AuthRepository _authRepository;

  // GoogleSignIn instance — configure clientId in google-services.json / Info.plist
  final GoogleSignIn _googleSignIn = GoogleSignIn(
    scopes: ['email', 'profile'],
  );

  AuthBloc({required AuthRepository authRepository})
      : _authRepository = authRepository,
        super(const AuthInitial()) {
    on<AppStarted>(_onAppStarted);
    on<LoginRequested>(_onLoginRequested);
    on<RegisterRequested>(_onRegisterRequested);
    on<GoogleLoginRequested>(_onGoogleLoginRequested);
    on<SendOtpRequested>(_onSendOtpRequested);
    on<OtpVerifyRequested>(_onOtpVerifyRequested);
    on<LogoutRequested>(_onLogoutRequested);
  }

  // ---------------------------------------------------------------------------
  // AppStarted — check for an existing session on launch
  // ---------------------------------------------------------------------------
  Future<void> _onAppStarted(
      AppStarted event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      final token = await _authRepository.getSavedToken();
      if (token == null || token.isEmpty) {
        emit(const AuthUnauthenticated());
        return;
      }

      final user = await _authRepository.getCurrentUser();
      if (user != null) {
        emit(AuthAuthenticated(user: user));
      } else {
        await _authRepository.clearTokens();
        emit(const AuthUnauthenticated());
      }
    } catch (_) {
      emit(const AuthUnauthenticated());
    }
  }

  // ---------------------------------------------------------------------------
  // LoginRequested
  // ---------------------------------------------------------------------------
  Future<void> _onLoginRequested(
      LoginRequested event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      final result =
          await _authRepository.login(event.email, event.password);
      emit(AuthAuthenticated(user: result.user));
    } catch (e) {
      emit(AuthError(message: e.toString().replaceFirst('Exception: ', '')));
    }
  }

  // ---------------------------------------------------------------------------
  // RegisterRequested
  // ---------------------------------------------------------------------------
  Future<void> _onRegisterRequested(
      RegisterRequested event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      final result = await _authRepository.register(
          event.fullName, event.email, event.password);
      emit(AuthAuthenticated(user: result.user));
    } catch (e) {
      emit(AuthError(message: e.toString().replaceFirst('Exception: ', '')));
    }
  }

  // ---------------------------------------------------------------------------
  // GoogleLoginRequested
  // ---------------------------------------------------------------------------
  Future<void> _onGoogleLoginRequested(
      GoogleLoginRequested event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      // NOTE: Google Sign-In requires valid google-services.json / Info.plist.
      // Until those are configured, this will throw and we catch it below.
      final googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        // User cancelled the sign-in flow
        emit(const AuthUnauthenticated());
        return;
      }

      final googleAuth = await googleUser.authentication;
      final idToken = googleAuth.idToken;

      if (idToken == null) {
        emit(const AuthError(
            message: 'Google Sign-In: ID token alınamadı'));
        return;
      }

      final result = await _authRepository.loginWithGoogle(idToken);
      emit(AuthAuthenticated(user: result.user));
    } catch (e) {
      // Google Sign-In yakında aktif olacak — yapılandırma tamamlanınca kullanılabilir
      emit(const AuthError(
          message: 'Google Sign-In yakında aktif olacak'));
    }
  }

  // ---------------------------------------------------------------------------
  // SendOtpRequested
  // ---------------------------------------------------------------------------
  Future<void> _onSendOtpRequested(
      SendOtpRequested event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      await _authRepository.sendOtp(event.phoneNumber);
      emit(OtpSent(phoneNumber: event.phoneNumber));
    } catch (e) {
      emit(AuthError(message: e.toString().replaceFirst('Exception: ', '')));
    }
  }

  // ---------------------------------------------------------------------------
  // OtpVerifyRequested
  // ---------------------------------------------------------------------------
  Future<void> _onOtpVerifyRequested(
      OtpVerifyRequested event, Emitter<AuthState> emit) async {
    emit(const AuthLoading());
    try {
      final result = await _authRepository.verifyOtp(
          event.phoneNumber, event.code);
      emit(AuthAuthenticated(user: result.user));
    } catch (e) {
      emit(AuthError(message: e.toString().replaceFirst('Exception: ', '')));
    }
  }

  // ---------------------------------------------------------------------------
  // LogoutRequested
  // ---------------------------------------------------------------------------
  Future<void> _onLogoutRequested(
      LogoutRequested event, Emitter<AuthState> emit) async {
    try {
      final refreshToken =
          await _authRepository.getSavedRefreshToken() ?? '';
      await _authRepository.logout(refreshToken);
    } catch (_) {
      // Ensure local tokens are cleared even if server-side logout fails
      await _authRepository.clearTokens();
    }

    try {
      await _googleSignIn.signOut();
    } catch (_) {
      // Ignore Google sign-out errors
    }

    emit(const AuthUnauthenticated());
  }
}
