import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../viewmodel/login_view_model.dart';

class LoginView extends ConsumerWidget {
  const LoginView({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(loginViewModelProvider);
    final viewModel = ref.read(loginViewModelProvider.notifier);

    return Scaffold(
      backgroundColor: const Color(0xFF102733),
      body: Column(
        children: [
          // 🖼 Banner image
          SizedBox(
            height: MediaQuery.of(context).size.height * 0.35,
            width: double.infinity,
            child: Image.asset(
              'assets/images/loginpageback.png',
              fit: BoxFit.cover,
            ),
          ),
Container(
  height: 1,
  width: double.infinity,
  color: Color(0xFFF9D65D),
),
          // 🧾 Login Form
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(horizontal: 32),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const SizedBox(height: 20),
                  const Text(
                    'Welcome to DrillingCore',
                    style: TextStyle(
                      fontSize: 26,
                      fontWeight: FontWeight.bold,
                      color: Color(0xFFF9D65D),
                    ),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 30),

                  // Username
                  _buildInputField(
                    hint: 'Username',
                    value: state.username,
                    onChanged: viewModel.updateUsername,
                    isPassword: false,
                  ),
                  const SizedBox(height: 16),

                  // Password
                  _buildInputField(
                    hint: 'Password',
                    value: state.password,
                    onChanged: viewModel.updatePassword,
                    isPassword: true,
                  ),

                  const SizedBox(height: 8),

                  // Forgot password
                  GestureDetector(
                    onTap: viewModel.forgotPassword,
                    child: const Align(
                      alignment: Alignment.centerRight,
                      child: Text(
                        'Forgot password?',
                        style: TextStyle(fontSize: 14, color: Color(0xFFF9D65D)),
                      ),
                    ),
                  ),

                  const SizedBox(height: 24),

                  // Login Button
                  SizedBox(
                    height: 50,
                    child: ElevatedButton(
                      onPressed: state.isLoading ? null : viewModel.login,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF1F4E79),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(16),
                        ),
                      ),
                      child: state.isLoading
                          ? const CircularProgressIndicator(color: Colors.white)
                          : const Text('Login', style: TextStyle(fontSize: 18, color: Colors.white)),
                    ),
                  ),

                  const SizedBox(height: 60),

                  const Center(
                    child: Text(
                      '© 2025 DrillingCore',
                      style: TextStyle(fontSize: 12, color: Color(0xFFC1CAD1)),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildInputField({
    required String hint,
    required String value,
    required void Function(String) onChanged,
    required bool isPassword,
  }) {
    return Container(
      decoration: BoxDecoration(
        border: Border.all(color: const Color(0xFFC1CAD1)),
        borderRadius: BorderRadius.circular(8),
      ),
      padding: const EdgeInsets.symmetric(horizontal: 10),
      child: TextField(
        onChanged: onChanged,
        obscureText: isPassword,
        style: const TextStyle(color: Colors.white),
        decoration: InputDecoration(
          hintText: hint,
          hintStyle: const TextStyle(color: Color(0xFF8899AA)),
          border: InputBorder.none,
        ),
      ),
    );
  }
}
