import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import '../../../core/services/user_session.dart';
import 'package:go_router/go_router.dart';
import '../../../features/todo/viewmodel/todo_view_model.dart';
import '../../../features/auth/viewmodel/login_view_model.dart';
class ProfileView extends ConsumerStatefulWidget {
  const ProfileView({super.key});

  @override
  ConsumerState<ProfileView> createState() => _ProfileViewState();
}

class _ProfileViewState extends ConsumerState<ProfileView> {
  String fullName = '';
  String username = '';
  String language = 'en'; // ru or en

  @override
  void initState() {
    super.initState();
    _loadUserInfo();
  }

  Future<void> _loadUserInfo() async {
    final session = await UserSession().getSession();
    setState(() {
      fullName = session?['fullName'] ?? '';
      username = session?['username'] ?? '';
    });
  }

  void _changeLanguage(String newLang) {
    setState(() => language = newLang);
    // TODO: сохранить выбор в storage / apply locale
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Profile'),
        backgroundColor: Colors.teal,
        foregroundColor: Colors.white,
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            const SizedBox(height: 20),
            Center(
              child: CircleAvatar(
                radius: 40,
                backgroundColor: Colors.teal.shade200,
                child: const Icon(Icons.person, size: 40),
              ),
            ),
            const SizedBox(height: 16),
            Center(child: Text(fullName, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold))),
            Center(child: Text(username, style: const TextStyle(fontSize: 16, color: Colors.grey))),
            const SizedBox(height: 30),

            const Divider(),

           ListTile(
  leading: const Icon(Icons.language),
  title: const Text('Language'),
  trailing: DropdownButton<String>(
    value: language,
    onChanged: (value) {
  if (value != null) {
    _changeLanguage(value);
  }
},
    items: const [
      DropdownMenuItem(value: 'en', child: Text('English')),
      DropdownMenuItem(value: 'fr', child: Text('Français')),
    ],
  ),
),

            const Divider(),

            ListTile(
              leading: const Icon(Icons.lock_outline),
              title: const Text('Change Password'),
              onTap: () {
                // TODO: реализовать переход на смену пароля
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(content: Text('Change Password - coming soon')),
                );
              },
            ),

            const Divider(),

            ListTile(
              leading: const Icon(Icons.logout),
              title: const Text('Logout'),
              onTap: () async {
                await UserSession().logout();

                 // 🧹 сбрасываем все провайдеры
                 ref.invalidate(todoViewModelProvider);
                  ref.invalidate(loginViewModelProvider);
                if (mounted) context.go('/login');
              },
            ),
          ],
        ),
      ),
    );
  }
}
