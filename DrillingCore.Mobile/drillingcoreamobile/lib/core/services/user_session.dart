import 'package:shared_preferences/shared_preferences.dart';

class UserSession {
  static const _tokenKey = 'auth_token';
  static const _usernameKey = 'username';
  static const _roleKey = 'role';
  static const _userIdKey = 'user_id';

  Future<void> saveSession(Map<String, dynamic> response) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_tokenKey, response['token']);
    await prefs.setString(_usernameKey, response['username']);
    await prefs.setString(_roleKey, response['role']);
    await prefs.setInt(_userIdKey, response['userId']);
  }

  Future<Map<String, dynamic>?> getSession() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString(_tokenKey);
    final username = prefs.getString(_usernameKey);
    final role = prefs.getString(_roleKey);
    final userId = prefs.getInt(_userIdKey);

    if (token == null || userId == null) return null;

    return {
      'token': token,
      'username': username,
      'role': role,
      'userId': userId,
    };
  }

  Future<void> clearSession() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }
}
