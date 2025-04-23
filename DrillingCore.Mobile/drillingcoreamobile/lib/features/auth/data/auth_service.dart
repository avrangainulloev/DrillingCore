import 'package:dio/dio.dart';
import '../../../core/services/api_client.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

final authServiceProvider = Provider<AuthService>((ref) {
  return AuthService(ApiClient().dio);
});

class AuthService {
  final Dio _dio;

  AuthService(this._dio);

  Future<Map<String, dynamic>?> login(String username, String password) async {
    try {
      final response = await _dio.post('auth/mobile-login', data: {
        'username': username,
        'password': password,
      });
      // print("FINAL :  ${response.toString()}");

      return response.data; // должен вернуть токен, роль и т.д.
    } on DioException catch (e) {
      print("Login error: ${e.message}");
      return null;
    }
  }
}
