import 'package:spotfinder_app/features/auth/data/models/user_model.dart';

class AuthResultModel {
  final String accessToken;
  final String refreshToken;
  final DateTime accessTokenExpiry;
  final UserModel user;

  const AuthResultModel({
    required this.accessToken,
    required this.refreshToken,
    required this.accessTokenExpiry,
    required this.user,
  });

  factory AuthResultModel.fromJson(Map<String, dynamic> json) {
    return AuthResultModel(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      accessTokenExpiry: json['accessTokenExpiry'] != null
          ? DateTime.parse(json['accessTokenExpiry'] as String)
          : DateTime.now().add(const Duration(hours: 1)),
      user: UserModel.fromJson(json['user'] as Map<String, dynamic>),
    );
  }

  Map<String, dynamic> toJson() => {
        'accessToken': accessToken,
        'refreshToken': refreshToken,
        'accessTokenExpiry': accessTokenExpiry.toIso8601String(),
        'user': user.toJson(),
      };

  @override
  String toString() =>
      'AuthResultModel(accessToken: [redacted], user: $user, expiry: $accessTokenExpiry)';
}
