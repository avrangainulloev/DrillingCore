 

 
import 'package:drillingcoreamobile/core/services/user_session.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'core/router/app_router.dart';
import 'package:permission_handler/permission_handler.dart';
Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();

   await requestPermissions();
  final session = UserSession();
  final token = await session.getToken();

  final initialRoute = (token != null && token.isNotEmpty) ? '/home' : '/login';

  runApp(
    ProviderScope(
      child: DrillingCoreApp(initialRoute: initialRoute),
    ),
  );
}

Future<void> requestPermissions() async {
  await [
    Permission.camera,
    Permission.storage,
    Permission.photos,
  ].request();
}
class DrillingCoreApp extends StatelessWidget {
  final String initialRoute;

  const DrillingCoreApp({super.key, required this.initialRoute});
 @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'DrillingCore',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.teal),
        useMaterial3: true,
      ),
      routerConfig: AppRouter.getRouter(initialRoute),
      debugShowCheckedModeBanner: false,
    );
  }
}