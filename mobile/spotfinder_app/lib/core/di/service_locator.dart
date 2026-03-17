import 'package:dio/dio.dart';
import 'package:spotfinder_app/features/auth/data/repositories/auth_repository.dart';

/// Minimal service locator — no external DI package required.
/// Repositories and services are created as static singletons.
/// Replace with get_it or injectable in later iterations if needed.
class ServiceLocator {
  ServiceLocator._();

  static final Dio dio = Dio(
    BaseOptions(
      connectTimeout: const Duration(seconds: 15),
      receiveTimeout: const Duration(seconds: 15),
      sendTimeout: const Duration(seconds: 15),
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
    ),
  );

  static final AuthRepository authRepository =
      AuthRepository(dio: dio);
}
