class UserModel {
  final String id;
  final String email;
  final String? fullName;
  final String? avatarUrl;
  final String provider;
  final String role;

  const UserModel({
    required this.id,
    required this.email,
    this.fullName,
    this.avatarUrl,
    required this.provider,
    required this.role,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) => UserModel(
        id: json['id'] as String,
        email: json['email'] as String,
        fullName: json['fullName'] as String?,
        avatarUrl: json['avatarUrl'] as String?,
        provider: json['provider'] as String,
        role: json['role'] as String,
      );

  Map<String, dynamic> toJson() => {
        'id': id,
        'email': email,
        'fullName': fullName,
        'avatarUrl': avatarUrl,
        'provider': provider,
        'role': role,
      };

  UserModel copyWith({
    String? id,
    String? email,
    String? fullName,
    String? avatarUrl,
    String? provider,
    String? role,
  }) {
    return UserModel(
      id: id ?? this.id,
      email: email ?? this.email,
      fullName: fullName ?? this.fullName,
      avatarUrl: avatarUrl ?? this.avatarUrl,
      provider: provider ?? this.provider,
      role: role ?? this.role,
    );
  }

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is UserModel && runtimeType == other.runtimeType && id == other.id;

  @override
  int get hashCode => id.hashCode;

  @override
  String toString() =>
      'UserModel(id: $id, email: $email, fullName: $fullName, provider: $provider, role: $role)';
}
