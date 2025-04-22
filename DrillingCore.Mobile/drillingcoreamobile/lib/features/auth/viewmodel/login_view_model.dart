import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/user_session.dart';
import '../data/auth_service.dart';
import '../../../core/router/navigation_key.dart'; // ключ для навигации
import 'package:go_router/go_router.dart';

final loginViewModelProvider =
    StateNotifierProvider<LoginViewModel, LoginState>(
  (ref) => LoginViewModel(ref.read(authServiceProvider)),
);

class LoginState {
  final String username;
  final String password;
  final bool isLoading;

  const LoginState({
    this.username = 'admin',
    this.password = 'admin',
    this.isLoading = false,
  });

  LoginState copyWith({
    String? username,
    String? password,
    bool? isLoading,
  }) {
    return LoginState(
      username: username ?? this.username,
      password: password ?? this.password,
      isLoading: isLoading ?? this.isLoading,
    );
  }
}

class LoginViewModel extends StateNotifier<LoginState> {
  final AuthService _authService;
  final _session = UserSession();

  LoginViewModel(this._authService) : super(const LoginState());

  void updateUsername(String username) {
    state = state.copyWith(username: username);
  }

  void updatePassword(String password) {
    state = state.copyWith(password: password);
  }

  Future<void> login() async {
    state = state.copyWith(isLoading: true);

    final result = await _authService.login(state.username, state.password);
    state = state.copyWith(isLoading: false);
    print("Login REsponse : ${result.toString()}");
    if (result != null && result['isAuthenticated'] == true) {
      await _session.saveSession(result);

      // Навигация на HomeView
      GoRouter.of(navigatorKey.currentContext!).go('/home');
    } else {
      showDialog(
        context: navigatorKey.currentContext!,
        builder: (_) => AlertDialog(
          title: const Text('Login Failed'),
          content: const Text('Invalid username or password'),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(navigatorKey.currentContext!).pop(),
              child: const Text('OK'),
            ),
          ],
        ),
      );
    }
  }

  void forgotPassword() {
    showDialog(
      context: navigatorKey.currentContext!,
      builder: (_) => AlertDialog(
        title: const Text('Reset Password'),
        content: const Text('Reset functionality coming soon.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(navigatorKey.currentContext!).pop(),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }
}
