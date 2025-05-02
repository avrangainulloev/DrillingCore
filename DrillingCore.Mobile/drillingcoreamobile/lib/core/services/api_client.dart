import 'package:dio/dio.dart';
import 'package:drillingcoreamobile/core/router/navigation_key.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:go_router/go_router.dart';
class ApiClient {
  final Dio dio;
  final _storage = const FlutterSecureStorage();

  ApiClient()
  : dio = Dio(
      BaseOptions(
        baseUrl: 'http://10.0.0.80:5000/api/',
        connectTimeout: const Duration(seconds: 25),
        receiveTimeout: const Duration(seconds: 25),
        validateStatus: (status) {
          return status != null && status >= 200 && status < 500;
        },
      ),
    ) {
  dio.interceptors.add(
    InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await _storage.read(key: 'token');
        if (token != null && token.isNotEmpty) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        return handler.next(options);
      },
      onError: (DioException e, handler) async {
        if (e.response?.statusCode == 401) {
          await _storage.delete(key: 'token');
          navigatorKey.currentContext?.go('/login');
        }
        return handler.next(e);
      },
    ),
  );
}
}
final apiClientProvider = Provider<ApiClient>((ref) => ApiClient());
