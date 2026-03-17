import 'package:dio/dio.dart';
import 'package:hive/hive.dart';
import 'package:spotfinder_app/core/constants/api_constants.dart';
import 'package:spotfinder_app/core/constants/storage_keys.dart';
import 'package:spotfinder_app/features/auth/data/models/auth_result_model.dart';
import 'package:spotfinder_app/features/auth/data/models/user_model.dart';

class AuthRepository {
  final Dio _dio;
  static const String _boxName = 'auth_box';

  AuthRepository({required Dio dio}) : _dio = dio;

  Box get _box => Hive.box(_boxName);

  // ---------------------------------------------------------------------------
  // Login with email & password
  // ---------------------------------------------------------------------------
  Future<AuthResultModel> login(String email, String password) async {
    try {
      final response = await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/login',
        data: {'email': email, 'password': password},
      );
      final result = AuthResultModel.fromJson(
          response.data as Map<String, dynamic>);
      await saveTokens(result);
      return result;
    } on DioException catch (e) {
      throw _handleDioError(e);
    }
  }

  // ---------------------------------------------------------------------------
  // Register with name, email & password
  // ---------------------------------------------------------------------------
  Future<AuthResultModel> register(
      String fullName, String email, String password) async {
    try {
      final response = await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/register',
        data: {'fullName': fullName, 'email': email, 'password': password},
      );
      final result = AuthResultModel.fromJson(
          response.data as Map<String, dynamic>);
      await saveTokens(result);
      return result;
    } on DioException catch (e) {
      throw _handleDioError(e);
    }
  }

  // ---------------------------------------------------------------------------
  // Login with Google ID token
  // ---------------------------------------------------------------------------
  Future<AuthResultModel> loginWithGoogle(String idToken) async {
    try {
      final response = await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/google',
        data: {'idToken': idToken},
      );
      final result = AuthResultModel.fromJson(
          response.data as Map<String, dynamic>);
      await saveTokens(result);
      return result;
    } on DioException catch (e) {
      throw _handleDioError(e);
    }
  }

  // ---------------------------------------------------------------------------
  // Send OTP to phone number
  // ---------------------------------------------------------------------------
  Future<void> sendOtp(String phoneNumber) async {
    try {
      await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/otp/send',
        data: {'phoneNumber': phoneNumber},
      );
    } on DioException catch (e) {
      throw _handleDioError(e);
    }
  }

  // ---------------------------------------------------------------------------
  // Verify OTP code
  // ---------------------------------------------------------------------------
  Future<AuthResultModel> verifyOtp(
      String phoneNumber, String code) async {
    try {
      final response = await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/otp/verify',
        data: {'phoneNumber': phoneNumber, 'code': code},
      );
      final result = AuthResultModel.fromJson(
          response.data as Map<String, dynamic>);
      await saveTokens(result);
      return result;
    } on DioException catch (e) {
      throw _handleDioError(e);
    }
  }

  // ---------------------------------------------------------------------------
  // Logout
  // ---------------------------------------------------------------------------
  Future<void> logout(String refreshToken) async {
    try {
      final token = await getSavedToken();
      await _dio.post(
        '${ApiConstants.authBaseUrl}/api/v1/auth/logout',
        data: {'refreshToken': refreshToken},
        options: Options(
          headers: {
            if (token != null) 'Authorization': 'Bearer $token',
          },
        ),
      );
    } on DioException catch (_) {
      // Ignore logout errors — clear local tokens regardless
    } finally {
      await clearTokens();
    }
  }

  // ---------------------------------------------------------------------------
  // Get current user from API
  // ---------------------------------------------------------------------------
  Future<UserModel?> getCurrentUser() async {
    final token = await getSavedToken();
    if (token == null) return null;

    try {
      final response = await _dio.get(
        '${ApiConstants.authBaseUrl}/api/v1/auth/me',
        options: Options(
          headers: {'Authorization': 'Bearer $token'},
        ),
      );
      return UserModel.fromJson(response.data as Map<String, dynamic>);
    } on DioException catch (_) {
      return null;
    }
  }

  // ---------------------------------------------------------------------------
  // Token persistence helpers
  // ---------------------------------------------------------------------------
  Future<String?> getSavedToken() async {
    final token = _box.get(StorageKeys.accessToken);
    return token as String?;
  }

  Future<void> saveTokens(AuthResultModel result) async {
    await _box.put(StorageKeys.accessToken, result.accessToken);
    await _box.put(StorageKeys.refreshToken, result.refreshToken);
    await _box.put(StorageKeys.userId, result.user.id);
    await _box.put(StorageKeys.userEmail, result.user.email);
    await _box.put(StorageKeys.userRole, result.user.role);
  }

  Future<void> clearTokens() async {
    await _box.delete(StorageKeys.accessToken);
    await _box.delete(StorageKeys.refreshToken);
    await _box.delete(StorageKeys.userId);
    await _box.delete(StorageKeys.userEmail);
    await _box.delete(StorageKeys.userRole);
  }

  Future<String?> getSavedRefreshToken() async {
    final token = _box.get(StorageKeys.refreshToken);
    return token as String?;
  }

  // ---------------------------------------------------------------------------
  // Internal error handler
  // ---------------------------------------------------------------------------
  Exception _handleDioError(DioException e) {
    final statusCode = e.response?.statusCode;
    final responseData = e.response?.data;

    String message = 'Bir hata oluştu';

    if (responseData is Map<String, dynamic>) {
      message = responseData['message'] as String? ??
          responseData['error'] as String? ??
          message;
    }

    if (statusCode == 401) {
      return Exception('E-posta veya şifre hatalı');
    } else if (statusCode == 409) {
      return Exception('Bu e-posta adresi zaten kullanılıyor');
    } else if (statusCode == 422) {
      return Exception('Geçersiz istek: $message');
    } else if (e.type == DioExceptionType.connectionTimeout ||
        e.type == DioExceptionType.receiveTimeout) {
      return Exception('Bağlantı zaman aşımına uğradı');
    } else if (e.type == DioExceptionType.connectionError) {
      return Exception('Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.');
    }

    return Exception(message);
  }
}
