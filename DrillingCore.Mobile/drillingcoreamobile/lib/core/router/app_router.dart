import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import '../../../features/auth/presentation/login_view.dart';
import '../../../features/home/presentation/home_view.dart';
import '../../features/forms/presentation/drill_inspection_page.dart'; // 👈 временный экран
import 'navigation_key.dart';

class AppRouter {
  static final router = GoRouter(
    navigatorKey: navigatorKey,
    initialLocation: '/login',
    routes: [
      GoRoute(
        path: '/login',
        builder: (context, state) => const LoginView(),
      ),
      GoRoute(
        path: '/home',
        builder: (context, state) => const HomeView(),
      ),
      GoRoute(
        path: '/form',
        builder: (context, state) {
          final formId = int.tryParse(state.uri.queryParameters['formId'] ?? '');
          final formTypeId = int.tryParse(state.uri.queryParameters['formTypeId'] ?? '');
          final projectId = int.tryParse(state.uri.queryParameters['projectId'] ?? '');

          if (formId == null || formTypeId == null || projectId == null) {
      return const Scaffold(body: Center(child: Text('Invalid form parameters')));
    }

    return DrillInspectionPage(
      formId: formId,
      formTypeId: formTypeId,
      projectId: projectId,
    );
        },
      ),
    ],
  );
}
